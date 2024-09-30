using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Phase4.Models;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace Phase4.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly HttpClient httpClient;

        public AdminController(HttpClient httpClient) { 
            
            this.httpClient = httpClient;
        }

        public IActionResult Index(string email)
        {
            if (email == null) {
                var userEmail = User.FindFirstValue("UserEmail");
                ViewBag.Email = userEmail;
                return View();
            }

            ViewBag.Email = email; 
            return View();
        }


        public async Task<IActionResult> RegisterRequest() {
            var response = await httpClient.GetAsync("https://localhost:7252/Phase4/getSuppliers");

            if (response.IsSuccessStatusCode)
            {
                
                var suppliers = await response.Content.ReadAsAsync<List<Suppliers>>();

                return View(suppliers); 
            }
            else
            {
               
                return View("Error");
            }
            
        }
        public async Task<IActionResult> ApproveSupplier(int id)
        {
            var response = await httpClient.PostAsync($"https://localhost:7252/Phase4/approveSupplier/{id}", null);

            if (response.IsSuccessStatusCode)
            {
                // Redirect to a success page or refresh the list of pending registrations
                return RedirectToAction("RegisterRequest");
            }
            else
            {
                // Handle error
                return View("Error");
            }
        }

        public async Task<IActionResult> RejectSupplier(int id)
        {
            var response = await httpClient.PostAsync($"https://localhost:7252/Phase4/rejectSupplier/{id}", null);

            if (response.IsSuccessStatusCode)
            {
                // Redirect to a success page or refresh the list of pending registrations
                return RedirectToAction("RegisterRequest");
            }
            else
            {
                // Handle error
                return View("Error");
            }
        }





        public async Task<IActionResult> GetRegSupplier()
        {
            var response = await httpClient.GetAsync("https://localhost:7252/Phase4/getRegSupplier");

            if (response.IsSuccessStatusCode)
            {

                var suppliers = await response.Content.ReadFromJsonAsync<List<Suppliers>>();

                return View("GetRegSupplier", suppliers);
            }
            else
            {

                return Ok("Error");
            }
        }

        public async Task<IActionResult> DisapproveSupplier(int supplierId)
        {
            string url = $"https://localhost:7252/Phase4/DisapproveSupplier?supplierId={supplierId}";
            var response = await httpClient.GetAsync(url);
          
            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("GetRegSupplier");
            }
            else
            {
                return Ok("Error");
            }

        }
        public async Task<IActionResult> ListSupplierPackages(int supplierId)
        {
            string url = $"https://localhost:7252/Phase4/ListSupplierPackages?supplierId={supplierId}";
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var supplierPackages = await response.Content.ReadFromJsonAsync<List<Packages>>();
                ViewBag.SupplierId = supplierId;

                // Check if TempData has the message and pass it to ViewBag
                if (TempData["ResultMessage"] != null)
                {
                    ViewBag.ResultMessage = TempData["ResultMessage"].ToString();
                }

                return View("ListSupplierPackages", supplierPackages);
            }
            else
            {
                return Ok("Error");
            }
        }


        public async Task<IActionResult> ApprovePackage(int packageId, int supllierId)
        {
            string url = $"https://localhost:7252/Phase4/ApprovePackage?packageId={packageId}";
            var response = await httpClient.GetAsync(url);
            //return Ok(supllierId);
            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("ListSupplierPackages", new { supplierId = supllierId });
            }
            else
            {
                return Ok("Error");
            }
        }
        public async Task<IActionResult> AdminUserWishList(AdminUserWishList packspec, int supllierId)
        {
            var jsonSupplier = JsonSerializer.Serialize(packspec);
            var content = new StringContent(jsonSupplier, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://localhost:7252/Phase4/AdminUserWishList", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                TempData["ResultMessage"] = responseContent;
            }
            else
            {
                TempData["ResultMessage"] = "Error";
            }

            return RedirectToAction("ListSupplierPackages", new { supplierId = supllierId });
        }



        public async Task<IActionResult> AdminPrice(int packageId, int adminPrice, int supplierId)
        {
            string url = $"https://localhost:7252/Phase4/AdminPrice?packageId={packageId}&adminPrice={adminPrice}";
            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListSupplierPackages", new { supplierId = supplierId });
            }
            else
            {

                return Ok("Error");
            }

        }

    }
}
