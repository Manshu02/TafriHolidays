using Microsoft.AspNetCore.Mvc;
using Phase4.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace Phase4.Controllers
{
    [Authorize(Policy = "SupplierOnly")]
    public class SupplierController : Controller
    {
        private readonly HttpClient httpClient;

        public SupplierController(HttpClient httpC)
        {
            httpClient = httpC;
        }
        public IActionResult Index()
        {
            var supplierId = User.FindFirst("UserID")?.Value;

            // Pass SupplierID to the view
            ViewBag.SupplierID = supplierId;

            return View();
        }
       
        public async Task<IActionResult> CheckLogin(string email) {

            var response = await httpClient.GetAsync($"https://localhost:7252/Phase4/CheckApproveRequest/{email}");
            if (!response.IsSuccessStatusCode)
            {
                
                return StatusCode((int)response.StatusCode, "Error fetching supplier registration status");
            }

            var statusMessage = await response.Content.ReadAsStringAsync();

            if (statusMessage == "approved")
            {
                
                return RedirectToAction("Index"); 
            }
            else if (statusMessage == "not approved")
            {
                string msg = "Wait for approval from Admin";
                return View("~/Views/Wait/Success.cshtml", msg);

            }
            else
            {
                // Handle unexpected status
                return BadRequest("Unexpected registration status");
            }
        }
        public IActionResult PackageRegister()
        {
            // Check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LoginPage", "Login");
            }

            // Retrieve the UserID claim
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int supplierId))
            {
                // Handle case where UserID claim is missing or invalid
                return RedirectToAction("LoginPage", "Login");
            }

            var package = new Packages
            {
                SupplierID = supplierId
            };

            return View(package);
        }


        public IActionResult DisplayPackage()
        {

            // Check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LoginPage", "Login");
            }

            // Retrieve the UserID claim
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int supplierId))
            {
                // Handle case where UserID claim is missing or invalid
                return RedirectToAction("LoginPage", "Login");
            }


            

            return View(supplierId);
        }



        public IActionResult EditPackage()
        {
            // Check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LoginPage", "Login");
            }

            // Retrieve the UserID claim
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int supplierId))
            {
                // Handle case where UserID claim is missing or invalid
                return RedirectToAction("LoginPage", "Login");
            }

          

            return View(supplierId);
        }


        public IActionResult Success()
        {
            return View();
        }
        public IActionResult Error() { 
            
            return View();
        }

    }
}
