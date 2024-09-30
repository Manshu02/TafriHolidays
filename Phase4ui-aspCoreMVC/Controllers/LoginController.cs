using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Phase4.Models;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Phase4.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient httpClient;

        public LoginController(HttpClient httpC)
        {
            httpClient = httpC;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LoginPage()
        {
            // Check if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                // Retrieve the UserType claim
                var userType = User.FindFirstValue("UserType");
                var userEmail = User.FindFirstValue("UserEmail");

                // Redirect to the appropriate dashboard based on the user type
                if (userType == "admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (userType == "supplier")
                {
                    return RedirectToAction("CheckLogin", "Supplier", new { email = userEmail });
                }
                else if (userType == "user")
                {
                    
                    var userIdClaim = User.FindFirst("UserID")?.Value;
                    int userId = int.Parse(userIdClaim);


                    return RedirectToAction("CheckLogin", "User", new { email = userEmail });
                }
            }

            // If the user is not authenticated, show the login page
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> LoginProcess(Login login)
        {
            if (login == null)
            {
                return BadRequest("Invalid login request.");
            }

            // Fetch login credentials from API
            var response = await httpClient.GetAsync($"https://localhost:7252/Phase4/getLogin/{login.Email}");

            if (response.IsSuccessStatusCode)
            {
                var storedLogin = await response.Content.ReadAsAsync<Login>();

                if (storedLogin != null && storedLogin.Password == login.Password) // Ideally, use hashed passwords and compare securely
                {


                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, login.Email),
                new Claim("UserID", storedLogin.Id.ToString()),
                new Claim("UserType", storedLogin.Type),
                new Claim("UserEmail",storedLogin.Email)
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);


                    // Store Supplier ID and Type in Session (or User/Admin ID and Type)
                    //HttpContext.Session.SetInt32("UserID", storedLogin.Id);
                    //HttpContext.Session.SetString("UserType", storedLogin.Type);



                    if (storedLogin.Type == "user")
                    {
                        return RedirectToAction("CheckLogin", "User", new { email = login.Email });
                    }
                    else if (storedLogin.Type == "supplier")
                    {
                        return RedirectToAction("CheckLogin", "Supplier", new { email = login.Email });
                    }
                    else if (storedLogin.Type == "admin")
                    {
                        return RedirectToAction("Index", "Admin", new { email = login.Email });
                    }
                }
            }

            return View("LoginPage");
        }

        public async Task<IActionResult> Logout()
        {
            // Sign the user out of the authentication scheme
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Clear any remaining session data
            HttpContext.Session.Clear();

            // Redirect to the login page or home page after logout
            return RedirectToAction("LoginPage");
        }
        public IActionResult SupplierAddRegister()
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SupplierRegister(Suppliers supplier)
        {
            if (supplier == null)
            {
                return BadRequest("Supplier is null.");
            }

            var jsonSupplier = JsonSerializer.Serialize(supplier);
            var content = new StringContent(jsonSupplier, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://localhost:7252/Phase4/setSupplier", content);
            Console.WriteLine(response.Content);

            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("LoginPage", "Login");  // Redirect to a success page or handle accordingly
            }
            else
            {

                return View("Error", responseContent);  // Redirect to an error page or handle accordingly
            }
        }

        public IActionResult AdminAddRegister()
        {

            return View();
        }
        public async Task<IActionResult> AdminRegister(Admins admin)
        {

            if (admin == null)
            {
                return BadRequest("User is null.");
            }

            var jsonSupplier = JsonSerializer.Serialize(admin);
            var content = new StringContent(jsonSupplier, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://localhost:7252/Phase4/setAdmin", content);
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
        public IActionResult UserAddRegister()
        {

            return View();
        }
        public async Task<IActionResult> UserRegister(Users user)
        {

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
    }
}
