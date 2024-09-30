using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Phase4.Models;
using Razorpay.Api;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Phase4API.Models;


namespace Phase4.Controllers
{
    [Authorize(Policy = "UserOnly")]


    public class UserController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly RazorpaySettings _razorpaySettings;
        private readonly IEmailService _emailService;
        public UserController(HttpClient httpC, IOptions<RazorpaySettings> razorpaySettings, IEmailService emailService)
        {
            _razorpaySettings = razorpaySettings.Value;
            httpClient = httpC;
            _emailService = emailService;

        }
       

        public IActionResult AddRegister()
        {

            return View();
        }
        public async Task<IActionResult> Register(Users user) {

            if (user == null)
            {
                return BadRequest("User is null.");
            }

            var jsonSupplier = JsonSerializer.Serialize(user);
            var content = new StringContent(jsonSupplier, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://localhost:7252/Phase4/setUser", content);
            Console.WriteLine(response.Content);

            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("LoginPage", "Login");
            }
            else
            {
                // Log the error or handle accordingly
                return View("Error", responseContent);  // Redirect to an error page or handle accordingly
            }
        }

        public async Task<IActionResult> CheckLogin(string email)
        {
            var response = await httpClient.GetAsync($"https://localhost:7252/Phase4/CheckLoginTime/{email}");

            if (response.IsSuccessStatusCode)
            {
                var loginCheck = await response.Content.ReadAsAsync<LoginCheckResponse>();

                if (loginCheck.CanLogin)
                {
                    var userIdClaim = User.FindFirst("UserID")?.Value;
                    int userId = int.Parse(userIdClaim);


                    ViewBag.UserId = userId;
                    return RedirectToAction("Index",new {userId=userId}); // Redirect to user dashboard or appropriate view
                }
                else
                {
                    ViewBag.TimeLeft = loginCheck.TimeLeft;
                    return View("~/Views/Wait/WaitForLogin.cshtml"); // Show the time left to log in
                }
            }
            else
            {
                return View("Error"); // Show error view if the API call fails
            }
        }



        public class LoginCheckResponse
        {
            public bool CanLogin { get; set; }
            public string TimeLeft { get; set; }
        }


        public async Task<IActionResult> ListPackages()
        {   
           
            var response = await httpClient.GetAsync("https://localhost:7252/Phase4/ListPackages");
            if (response.IsSuccessStatusCode)
            {

                var Listpackages = await response.Content.ReadFromJsonAsync<List<Packages>>();

                var userIdClaim = User.FindFirst("UserID")?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                {
                    ViewBag.UserId = userId;
                }
                else
                {
                    // Handle the case where the claim is not found or cannot be parsed
                    ViewBag.UserId = 0; // Or handle as appropriate
                }

                return View("ListPackages", Listpackages);
            }
            else
            {

                return Ok("Error");
            }
        }



        public async Task<IActionResult> BuyPackage(Payments payments)
        {
            var referrer = Request.Headers["Referer"].ToString();
            if (!referrer.Contains("/User/ListPackages")&&!referrer.Contains("/User/GetCartPackages")&&!referrer.Contains("User/ListRecommendedPackages"))
            {
                return RedirectToAction("AccessDenied", "Home"); // Redirect to a safe page or show an error
            }
            // Calculate total price
            payments.packagePrice = payments.packagePrice * payments.count;
            payments.AdminPrice = payments.AdminPrice * payments.count;

            // Create Razorpay Order
            var client = new RazorpayClient(_razorpaySettings.Key, _razorpaySettings.Secret);
            var options = new Dictionary<string, object>
    {
        { "amount", payments.AdminPrice*100  },  // amount in the smallest currency unit (paise)
        { "currency", "INR" },
        { "receipt", Guid.NewGuid().ToString() },
        { "payment_capture", 1 } // auto-capture
    };

            var order = client.Order.Create(options);

            // Save the order ID to pass it to the view
            ViewBag.RazorpayOrderId = order["id"].ToString();
            ViewBag.RazorpayKey = _razorpaySettings.Key;
            ViewBag.PackageId = payments.packageId;
            ViewBag.Count = payments.count;

            // Return the view with Razorpay payment form
            return View(payments);
        }

        [HttpPost]
        public async Task<IActionResult> PaymentSuccess(
        string razorpay_payment_id,
        string razorpay_order_id,
        string razorpay_signature,
        int packageId,
        int count,
        int packagePrice,
        int adminPrice)
        {
            var client = new RazorpayClient(_razorpaySettings.Key, _razorpaySettings.Secret);

            var attributes = new Dictionary<string, string>
    {
        { "razorpay_payment_id", razorpay_payment_id },
        { "razorpay_order_id", razorpay_order_id },
        { "razorpay_signature", razorpay_signature }
    };

            try
            {
                // Verify the payment signature
                Utils.verifyPaymentSignature(attributes);

                var useridclaim = User.FindFirst("userid")?.Value;
                if (useridclaim == null)
                {
                    return Ok("User ID claim not found.");
                }

                int usId = int.Parse(useridclaim);// change name

                // Call the new API to get the user's details based on userId
                var userApiUrl = $"https://localhost:7252/Phase4/user?userid={usId}";
                var userResponse = await httpClient.GetAsync(userApiUrl);

                if (!userResponse.IsSuccessStatusCode)
                {
                    return Ok($"Error fetching user details: {userResponse.ReasonPhrase}");
                }

                var user = await userResponse.Content.ReadFromJsonAsync<Users>();

                if (user == null)
                {
                    return Ok("User deserialization failed.");
                }

                var userPhoneNumber = user.UserContact.ToString();

                var payments = new Payments
                {
                    packageId = packageId,
                    userId = usId,
                    packagePrice = packagePrice,
                    AdminPrice = adminPrice,
                    count = count,
                    date = DateOnly.FromDateTime(DateTime.Now)
                };

                // Call your API to save the payment record
                var jsonSupplier = JsonSerializer.Serialize(payments);
                var content = new StringContent(jsonSupplier, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://localhost:7252/Phase4/BuyPackage", content);


                

                if (response.IsSuccessStatusCode)
                {
                    var userEmail = User.FindFirst("UserEmail")?.Value;
                    string logoUrl = "https://cdn-icons-png.flaticon.com/128/1358/1358770.png"; // Replace with actual URL
                    string subject = "Purchase Confirmation";
                    string message = $@"
                    <div style='text-align:center;'>
                     <img src='{logoUrl}' alt='Logo' style='width:150px;height:auto;' />
                     <h2>Thank you for your purchase of ₹{adminPrice}</h2>
                    <p>Your order has been placed and will be processed soon.</p>
                     </div>";

                    try
                    {
                        await _emailService.SendEmailAsync(userEmail, subject, message);
                        return RedirectToAction("UpdateUsedCount", new { packageId = packageId, count = count });
                    }
                    catch (Exception ex)
                    {
                        // Log and handle the exception
                        return StatusCode(500, $"Error completing purchase: {ex.Message}");
                    }
                   

                }
                else
                {
                    return Ok($"Error processing payment: {response.ReasonPhrase}");
                }
               
                
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Exception: {ex.Message}");
                return Ok("Payment verification failed.");
            }
        }


        public async Task<IActionResult> UpdateUsedCount(int packageId,int count)
        {
            string url = $"https://localhost:7252/Phase4/UpdateUsedCount?PackageID={packageId}&count={count}";
            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListPackages");
            }
            else
            {

                return Ok("Error");
            }
        }


        public async Task<IActionResult> AddToCart(Cart cart)
        {
            var jsonSupplier = JsonSerializer.Serialize(cart);
            var content = new StringContent(jsonSupplier, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://localhost:7252/Phase4/AddToCart", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (responseContent == "Already In Wishlist")
            {
                ViewBag.Message = "This package is already in your cart.";
                return RedirectToAction("ListPackages");
            }
            else if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetCartPackages", new { userId = cart.userId });
            }
            else
            {
                ViewBag.Message = "An error occurred while adding the package to your cart.";
                return RedirectToAction("ListPackages");
            }
        }


        public async Task<IActionResult> GetCartPackages(int userId)
            
        {


            
                var userIdClaim = User.FindFirst("UserID")?.Value;
                if (int.TryParse(userIdClaim, out int Id))
                {
                    userId = Id;
                }
                else
                {
                    // Handle the case where the claim is not found or cannot be parsed
                    userId = 0 ; // Or handle as appropriate
                }
            
            

            var response = await httpClient.GetAsync($"https://localhost:7252/Phase4/GetCartPackages?userid={userId}");
            if (response.IsSuccessStatusCode)
            {

                var Listpackages = await response.Content.ReadFromJsonAsync<List<Packages>>();
                ViewBag.UserId = userId;
                return View("CartPackages", Listpackages);
            }
            else
            {

                return Ok("Error");
            }
        }


        public async Task<IActionResult> ListRecommendedPackages()

        {
            int userId;
            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (int.TryParse(userIdClaim, out int Id))
            {
                userId = Id;
            }
            else
            {
                // Handle the case where the claim is not found or cannot be parsed
                userId = 0; // Or handle as appropriate
            }
            var response = await httpClient.GetAsync($"https://localhost:7252/phase4/ListRecommendedPackages?userId={userId}");

            if (response.IsSuccessStatusCode)
            {

                var ListRecommendedpackages = await response.Content.ReadFromJsonAsync<List<PackageSpecificationViewModel>>();
                ViewBag.UserId = userId;

                return View("ListRecommendedPackages", ListRecommendedpackages);
            }
            else
            {

                return Ok("Error");
            }
        }
        public async Task<IActionResult> RemoveFromCart(int PackageID, int userid)
        {
            var response = await httpClient.GetAsync($"https://localhost:7252/Phase4/RemoveFromCart?PackageID={PackageID}&userid={userid}");
            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("GetCartPackages", userid);
            }
            else
            {

                return Ok("Error");
            }
        }
        public async Task<IActionResult> Index(int userId)
        {
            if (userId == null)
            {
                var useridclaim = User.FindFirst("userid")?.Value;
                 userId = int.Parse(useridclaim);


                
            }
            ViewBag.userid = userId;

            var response = await httpClient.GetAsync($"https://localhost:7252/Phase4/UserInfo?userid={userId}");
            if (response.IsSuccessStatusCode)
            {
                var userinfo = await response.Content.ReadFromJsonAsync<List<Users>>();
                return View("Index", userinfo);
            }
            else
            {

                return Ok("Error");
            }
        }
    }
}
