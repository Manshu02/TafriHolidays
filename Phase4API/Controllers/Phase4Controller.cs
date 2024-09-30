using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Phase4.Models;
using Phase4API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Phase4API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class Phase4Controller : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public Phase4Controller(ApplicationDbContext db)
        {
            dbContext = db;
        }
        [HttpPost("setSupplier")]
        public async Task<IActionResult> SetSupplier([FromBody] Suppliers supplier)
        {

            if (supplier == null)
            {
                return BadRequest("Admin is null.");
            }

            try
            {

                dbContext.Suppliers.Add(supplier);
                await dbContext.SaveChangesAsync();

                // Retrieve the AdminID after saving
                var SupplierId = await dbContext.Suppliers
                    .Where(l => l.SupplierEmail == supplier.SupplierEmail)
                    .Select(l => supplier.SupplierID)
                    .SingleOrDefaultAsync();

                if (SupplierId == default)
                {
                    return NotFound("Admin not found after saving.");
                }

                // Create the login entry
                var loginEntry = new Phase4.Models.Login
                {
                    Email = supplier.SupplierEmail,
                    Password = supplier.SupplierPassword,
                    Type = supplier.Type,
                    Id = SupplierId
                };

                dbContext.Login.Add(loginEntry);
                await dbContext.SaveChangesAsync(); // Save changes for the login entry

                return Ok("Supplier registered..redirecting to login");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("greetings")]
        public string test()
        {
            return "Hello";
        }

        [HttpGet("getSuppliers")]
        public async Task<ActionResult<List<Suppliers>>> GetPendingRegistrations()
        {
            var pendingSuppliers = await dbContext.Suppliers
                .Where(s => !s.RegisterationStatus) // Assuming IsRegistered is a boolean property
                .ToListAsync();

            return Ok(pendingSuppliers);
        }
        [HttpPost("approveSupplier/{id}")]
        public async Task<IActionResult> ApproveSupplier(int id)
        {
            var supplier = await dbContext.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound("Supplier not found.");
            }

            supplier.RegisterationStatus = true; // Assuming this is the field that tracks approval
            await dbContext.SaveChangesAsync();

            return Ok("Supplier approved successfully.");
        }

        [HttpPost("rejectSupplier/{id}")]
        public async Task<IActionResult> RejectSupplier(int id)
        {
            var supplier = await dbContext.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound("Supplier not found.");
            }

            dbContext.Suppliers.Remove(supplier);
            await dbContext.SaveChangesAsync();

            return Ok("Supplier rejected successfully.");
        }

        [HttpGet("getProducer/{id}")]
        public async Task<ActionResult<Producer>> getProducer(int id)
        {

            var producer = await dbContext.Producer
            .Include(p => p.Products)
            .FirstOrDefaultAsync(p => p.ProducerId == id);


            if (producer == null)
            {
                return NotFound(); // Return 404 if producer not found
            }


            return Ok(producer);
        }
        [HttpGet("getProduct/{id}")]
        public async Task<ActionResult<Producer>> getProduct(int id)
        {

            var product = await dbContext.Product
            .Include(p => p.Producer)
            .FirstOrDefaultAsync(p => p.ProducerId == id);


            if (product == null)
            {
                return NotFound(); // Return 404 if producer not found
            }


            return Ok(product);
        }

        [HttpPost("setUser")]
        public async Task<IActionResult> SetUser([FromBody] Users user)
        {

            if (user == null)
            {
                return BadRequest("Admin is null.");
            }

            try
            {

                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();

                // Retrieve the AdminID after saving
                var UserId = await dbContext.Users
                    .Where(l => l.UserEmail == user.UserEmail)
                    .Select(l => l.UserID)
                    .SingleOrDefaultAsync();

                if (UserId == default)
                {
                    return NotFound("Admin not found after saving.");
                }

                // Create the login entry
                var loginEntry = new Phase4.Models.Login
                {
                    Email = user.UserEmail,
                    Password = user.UserPassword,
                    Type = user.Type,
                    Id = UserId
                };

                dbContext.Login.Add(loginEntry);
                await dbContext.SaveChangesAsync(); // Save changes for the login entry

                return Ok("Admin registered..redirecting to login");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("getLogin/{email}")]
        public async Task<ActionResult<Login>> GetLogin(string email)
        {
            var login = await dbContext.Login.SingleOrDefaultAsync(l => l.Email == email);
            if (login == null)
            {
                return NotFound("Login not found.");
            }

            return Ok(login);
        }

        [HttpGet("CheckLoginTime/{email}")]
        public async Task<IActionResult> CheckLoginTime(string email)
        {
            var user = await dbContext.Users.SingleOrDefaultAsync(l => l.UserEmail == email);

            if (user == null)
            {
                return NotFound();
            }

            // Assuming RegistrationTime is stored as UTC
            var registrationTimeUtc = user.RegistrationTime;
            var timeSinceRegistration = DateTime.UtcNow - registrationTimeUtc;

            // Ensure timeSinceRegistration is not negative
            if (timeSinceRegistration < TimeSpan.Zero)
            {
                timeSinceRegistration = TimeSpan.Zero;
            }

            if (timeSinceRegistration >= TimeSpan.FromMinutes(15))
            {
                return Ok(new { CanLogin = true });
            }
            else
            {
                var timeLeft = TimeSpan.FromMinutes(15) - timeSinceRegistration;
                return Ok(new { CanLogin = false, TimeLeft = timeLeft.ToString(@"hh\:mm\:ss") });
            }
        }
        [HttpGet("CheckApproveRequest/{email}")]
        public async Task<IActionResult> CheckApproveRequest(string email)
        {

            var supplier = await dbContext.Suppliers.SingleOrDefaultAsync(l => l.SupplierEmail == email);
            if (supplier == null)
            {
                return NotFound("Supplier not found");
            }

            // Determine the registration status message
            string statusMessage = supplier.RegisterationStatus == true ? "approved" : "not approved";

            return Ok(statusMessage);
        }

        [HttpPost("setAdmin")]
        public async Task<IActionResult> SetAdmin([FromBody] Admins admin)
        {
            if (admin == null)
            {
                return BadRequest("Admin is null.");
            }

            try
            {

                dbContext.Admins.Add(admin);
                await dbContext.SaveChangesAsync();

                // Retrieve the AdminID after saving
                var adminId = await dbContext.Admins
                    .Where(l => l.AdminEmail == admin.AdminEmail)
                    .Select(l => l.AdminID)
                    .SingleOrDefaultAsync();

                if (adminId == default)
                {
                    return NotFound("Admin not found after saving.");
                }

                // Create the login entry
                var loginEntry = new Phase4.Models.Login
                {
                    Email = admin.AdminEmail,
                    Password = admin.AdminPassword,
                    Type = admin.Type,
                    Id = adminId
                };

                dbContext.Login.Add(loginEntry);
                await dbContext.SaveChangesAsync(); // Save changes for the login entry

                return Ok("Admin registered..redirecting to login");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


        [HttpPost("setpackage")]
        public IActionResult SetPackage([FromBody] Packages package)
        {
            Console.WriteLine(package);
            if (package == null)
            {
                return BadRequest("Package is null.");
            }
            try
            {
                dbContext.Packages.Add(package);
                dbContext.SaveChanges();
                return Ok("Package registered");

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }


        }
        [HttpGet("getpackages/{id}")]
        public async Task<ActionResult<List<Packages>>> GetPackages(int id)
        {
            // Fetch packages where SupplierID matches the provided id
            var packages = await dbContext.Packages
                                         .Where(p => p.SupplierID == id)
                                         .ToListAsync();

            // No need to check for null as ToListAsync will return an empty list if no records found
            return Ok(packages);
        }

        [HttpPost("updatepackage/{packageId}")]
        public async Task<IActionResult> UpdatePackageStatus(int packageId, [FromBody] UpdatePackageRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Action))
            {
                return BadRequest("Invalid request");
            }

            // Retrieve the package from the database
            var package = await dbContext.Packages.FindAsync(packageId);
            if (package == null)
            {
                return NotFound("Package not found");
            }

            if (request.Action == "deactivate")
            {
                if (package.UsedCount > 0)
                {
                    return BadRequest("Cannot deactivate a package with used count greater than 0.");
                }
                package.IsActive = false;
                package.IsApproved = false;
            }
            else if (request.Action == "activate")
            {
                package.IsActive = true;

            }
            else
            {
                return BadRequest("Invalid action");
            }

            // Save changes to the database
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        public class UpdatePackageRequest
        {
            public string Action { get; set; }
        }

        [HttpGet("getpackage/{packageId}/{supplierId}")]
        public async Task<IActionResult> GetPackage(int packageId, int supplierId)
        {
            // Find the package including the SupplierID check
            var package = await dbContext.Packages
                .Where(p => p.PackageID == packageId && p.SupplierID == supplierId)
                .FirstOrDefaultAsync();

            if (package == null)
            {
                return NotFound("Package not found or does not belong to the specified supplier.");
            }

            return Ok(package);
        }


        [HttpPut("editPackage")]
        public async Task<IActionResult> EditPackage([FromBody] Packages package)
        {
            if (package?.PackageID == 0 || package.SupplierID == 0)// check packahe null
            {
                return BadRequest("Invalid package data.");
            }

            var existingPackage = await dbContext.Packages
                .FirstOrDefaultAsync(p => p.PackageID == package.PackageID && p.SupplierID == package.SupplierID);

            if (existingPackage == null)
            {
                return NotFound("Package not found or does not belong to the specified supplier.");
            }

            // Update the fields
            dbContext.Entry(existingPackage).CurrentValues.SetValues(package);

            // Save changes to the database
            try
            {
                await dbContext.SaveChangesAsync();
                return Ok("Package updated successfully.");// if any error occur and throw it and check in both catches?
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PackageExists(package.PackageID))
                {
                    return NotFound("Package no longer exists.");
                }
                throw;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private bool PackageExists(int id)
        {
            return dbContext.Packages.Any(e => e.PackageID == id);
        }

        [HttpGet("getRegSupplier")]
        public IActionResult GetRegSupplier()
        {
            var regSuppliers = from supplier in dbContext.Suppliers
                               where supplier.RegisterationStatus
                               select supplier;
            return Ok(regSuppliers.ToList());

        }


        [HttpGet("DisapproveSupplier")]
        public async Task<IActionResult> DisapproveSupplier(int supplierId)
        {
            var supplier = await dbContext.Suppliers
                .Where(s => s.SupplierID == supplierId)
                .FirstOrDefaultAsync();

            if (supplier != null)
            {
                // Update the RegistrationStatus
                supplier.RegisterationStatus = false; // Equivalent to 0

                // Save the changes to the database
                await dbContext.SaveChangesAsync();

                // Redirect or return a view as needed
                return Ok("success");
                //return RedirectToAction("GetRegSupplier","Admin");
            }
            return Ok("error");
        }

        [HttpGet("ListSupplierPackages")]
        public IActionResult ListSupplierPackages(int supplierId)
        {
            var listApprovedSupplier = from package in dbContext.Packages
                                       where package.SupplierID == supplierId
                                       select package;

            return Ok(listApprovedSupplier.ToList());

        }




        [HttpGet("ApprovePackage")]
        public async Task<IActionResult> ApprovePackage(int packageId)
        {
            var package = await dbContext.Packages
                .Where(p => p.PackageID == packageId)
                .FirstOrDefaultAsync();

            if (package != null)
            {
                package.IsApproved = true;
                package.IsActive = true;
                await dbContext.SaveChangesAsync();
                return Ok("Success");
            }
            return Ok("error");
        }

        [HttpPost("AdminUserWishList")]
        public async Task<IActionResult> PackageSpecification(AdminUserWishList packageSpecification)
        {
            if (ModelState.IsValid)
            {
                // Check if an entry with the same PackageID and UserID already exists
                var existingEntry = await dbContext.AdminUserWishList
                    .FirstOrDefaultAsync(w => w.PackageID == packageSpecification.PackageID && w.UserID == packageSpecification.UserID);

                if (existingEntry != null)
                {
                    // If the entry already exists, return an error or handle it as needed
                    return Ok("Package already assigned to this user");
                }

                // If no existing entry is found, add the new record
                dbContext.AdminUserWishList.Add(packageSpecification);
                await dbContext.SaveChangesAsync();
                return Ok("Package assigned successfully");
            }
            return Ok("error");
        }





        [HttpGet("AdminPrice")]
        public async Task<IActionResult> AdminPrice(int packageId, int adminPrice)
        {
            if (ModelState.IsValid)
            {
                var package = await dbContext.Packages.FindAsync(packageId);
                package.AdminPrice = adminPrice;
                await dbContext.SaveChangesAsync();

                return Ok("Success");
            }
            return Ok("error");
        }

        [HttpGet("ListPackages")]
        public async Task<IActionResult> ListPackages(int userId)
        {
            var packageList = from packages in dbContext.Packages
                              join sup in dbContext.Suppliers
                              on packages.SupplierID equals sup.SupplierID
                              where sup.RegisterationStatus == true
            && packages.IsActive == true
                              select packages;

            return Ok(await packageList.ToListAsync());
        }

        [HttpPost("BuyPackage")]
        public async Task<IActionResult> BuyPackage([FromBody] Payments payments)
        {
            if (ModelState.IsValid)
            {
                dbContext.Payments.Add(payments);
                await dbContext.SaveChangesAsync();
                return Ok("success");
            }
            return Ok("error");
        }

        [HttpGet("UpdateUsedCount")]
        public async Task<IActionResult> UpdateUsedCount(int packageId, int count)
        {
            var package = await dbContext.Packages.FindAsync(packageId);

            if (package == null)
            {
                return NotFound(); // Handle the case where the package is not found
            }

            if (package.UsedCount < package.TotalCount - 1)
            {
                package.UsedCount += count;

                // Save the changes to the database
                await dbContext.SaveChangesAsync();

                return Ok("Success");
            }
            else if (package.UsedCount == package.TotalCount - 1)
            {
                package.UsedCount += 1;
                package.IsActive = false;
                await dbContext.SaveChangesAsync();
                return Ok("Success");
            }
            else
            {
                return Ok("error");
            }
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(Cart cart)
        {
            if (ModelState.IsValid)
            {

                var existingCart = await dbContext.Cart
                    .FirstOrDefaultAsync(c => c.userId == cart.userId && c.packageId == cart.packageId);

                if (existingCart != null)
                {
                    return Ok("Already In Wishlist");
                }


                dbContext.Cart.Add(cart);
                await dbContext.SaveChangesAsync();
                return Ok("success");
            }
            return Ok("error");
        }


        [HttpGet("GetCartPackages")]
        public async Task<IActionResult> GetCartPackages(int userId)
        {
            var cartPackageList = from packages in dbContext.Packages
                                  join sup in dbContext.Suppliers
                                  on packages.SupplierID equals sup.SupplierID
                                  where sup.RegisterationStatus == true
            && packages.IsActive == true
            && dbContext.Cart.Any(cart => cart.userId == userId && cart.packageId == packages.PackageID)
                                  select packages;
            return Ok(await cartPackageList.ToListAsync());
        }



        [HttpGet("ListRecommendedPackages")]
        public async Task<IActionResult> ListRecommendedPackages(int userId)
        {
            var result = from ps in dbContext.AdminUserWishList
                         join p in dbContext.Packages
                         on ps.PackageID equals p.PackageID
                         join s in dbContext.Suppliers
                         on p.SupplierID equals s.SupplierID
                         where s.RegisterationStatus == true
            && p.IsActive == true
            && ps.UserID == userId
                         select new PackageSpecificationViewModel
                         {
                             PackageId = ps.PackageID,
                             UserId = ps.UserID,
                             FlightName = p.FlightName,
                             From = p.From,
                             To = p.To,
                             FlightType = p.FlightType,
                             AccName = p.AccName,
                             AccType = p.AccType,
                             AccAddress = p.AccAddress,
                             SSNames = p.SSNames,
                             Luxuary = p.Luxuary,
                             PackagePrice = p.PackagePrice,
                             AdminPrice = p.AdminPrice,
                             TotalCount = p.TotalCount,
                             UsedCount = p.UsedCount
                         };
            return Ok(await result.ToListAsync());
        }

        [HttpGet("RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCart(int PackageID, int userid)
        {
            var packageSpec = await dbContext.Cart
        .FirstOrDefaultAsync(p => p.packageId == PackageID && p.userId == userid);

            dbContext.Cart.Remove(packageSpec);

            await dbContext.SaveChangesAsync();
            return Ok("Success");
        }


        [HttpGet("UserInfo")]
        public async Task<IActionResult> UserInfo(int userid)
        {
            var userinfo = from user in dbContext.Users
                           where user.UserID == userid
                           select user;

            if (userinfo == null)
            {
                return NotFound("User not found.");
            }

            return Ok(userinfo);
        }

        [HttpGet("UserPurchasedPackages")]
        public async Task<IActionResult> UserPurchasedPackages(int userid)
        {
            var UserPackages = from pay in dbContext.Payments
                               join p in dbContext.Packages
                               on pay.packageId equals p.PackageID
                               where pay.userId == userid
                               select new UserPackageDTO
                               {
                                   PackageId = pay.packageId,
                                   UserId = pay.userId,
                                   PackagePrice = pay.AdminPrice,
                                   Count = pay.count,
                                   Date = pay.date,
                                   FlightName = p.FlightName,
                                   From = p.From,
                                   To = p.To,
                                   FlightType = p.FlightType,
                                   AccName = p.AccName,
                                   AccType = p.AccType,
                                   AccAddress = p.AccAddress,
                                   SSNames = p.SSNames,
                                   Luxuary = p.Luxuary
                               };
            return Ok(await UserPackages.ToListAsync());
        }

        [HttpGet("user")]
        public IActionResult GetUser(int userid)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.UserID == userid);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        [HttpGet("GetProfitData")]
        public IActionResult GetProfitData()
        {
            var data = dbContext.Payments
                .GroupBy(p => p.packageId)
                .Select(g => new
                {
                    PackageId = g.Key,
                    Profit = g.Sum(p => p.AdminPrice - p.packagePrice)
                })
                .ToList();

            return Ok(data);
        }

        [HttpGet("monthly-summary-prices")]
        public IActionResult GetMonthlySummaryWithPrices()
        {
            var monthlySummary = new[]
            {
        new
        {
            TotalPackagePrice = dbContext.Payments.Sum(p => p.packagePrice),  // Sum of all PackagePrice
            Profit = dbContext.Payments.Sum(p => p.AdminPrice) - dbContext.Payments.Sum(p => p.packagePrice)       // Sum of all AdminPrice
        }
    };

            return Ok(monthlySummary);
        }

        [HttpGet("monthly-summary")]
        public IActionResult GetMonthlySummary()
        {
            var monthlySummary = dbContext.Payments
                .GroupBy(p => new { p.date.Year, p.date.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalAdminPrice = g.Sum(p => p.AdminPrice)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            return Ok(monthlySummary);
        }
        [HttpGet("supplier-monthly-summary/{supplierId}")]
        public IActionResult SupplierGetMonthlySummary(int supplierId)
        {
            var monthlySummary = dbContext.Payments
                .Join(dbContext.Packages,
                      payment => payment.packageId,
                      package => package.PackageID,
                      (payment, package) => new { payment, package })
                .Where(p => p.package.SupplierID == supplierId)  // Filter by SupplierID
                .GroupBy(p => new { p.payment.date.Year, p.payment.date.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalPackagePrice = g.Sum(p => p.payment.packagePrice)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            return Ok(monthlySummary);
        }

        [HttpGet("total-package-price/{supplierId}")]
        public async Task<IActionResult> GetTotalPackagePrice(int supplierId)
        {
            var data = await dbContext.Payments
                .Join(dbContext.Packages,
                      payment => payment.packageId,
                      package => package.PackageID,
                      (payment, package) => new { payment, package })
                .Where(p => p.package.SupplierID == supplierId)  // Filter by SupplierID
                .GroupBy(p => p.payment.packageId)
                .Select(g => new
                {
                    packageId = g.Key,
                    totalPrice = g.Sum(p => p.payment.packagePrice)
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("suppliersinfo/{supplierId}")]
        public IActionResult GetSupplierById(int supplierId)
        {
            var supplier = dbContext.Suppliers.FirstOrDefault(s => s.SupplierID == supplierId);
            if (supplier == null)
            {
                return NotFound();
            }
            return Ok(supplier);
        }

        [HttpGet("supplier-package-counts/{supplierId}")]
        public IActionResult GetSupplierPackageCounts(int supplierId)
        {
            var totalPackages = dbContext.Packages.Count(p => p.SupplierID == supplierId);
            var activePackages = dbContext.Packages.Count(p => p.SupplierID == supplierId && p.IsActive);
            var approvedPackages = dbContext.Packages.Count(p => p.SupplierID == supplierId && p.IsApproved);

            return Ok(new
            {
                totalPackages = totalPackages,
                activePackages = activePackages,
                approvedPackages = approvedPackages
            });
        }


        [HttpGet("admindetail/{adminId}")]
        public IActionResult GetAdminDetails(int adminId)
        {
            var admin = dbContext.Admins.FirstOrDefault(a => a.AdminID == adminId);

            if (admin == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                admin.AdminID,
                admin.AdminName,
                admin.AdminContact,
                admin.AdminEmail
            });
        }

        [HttpGet("check-email")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            if (await dbContext.Suppliers.AnyAsync(s => s.SupplierEmail == email))
            {
                return Ok(new { exists = true });
            }

            return Ok(new { exists = false });
        }

        [HttpGet("check-user-email")]
        public async Task<IActionResult> CheckUserEmail(string email)
        {
            if (await dbContext.Users.AnyAsync(s => s.UserEmail == email))
            {
                return Ok(new { exists = true });
            }

            return Ok(new { exists = false });
        }


        [HttpGet("check-admin-email")]
        public async Task<IActionResult> CheckAdminEmail(string email)
        {
            if (await dbContext.Admins.AnyAsync(s => s.AdminEmail == email))
            {
                return Ok(new { exists = true });
            }

            return Ok(new { exists = false });
        }


        [AllowAnonymous]
        [HttpPost("Reactlogin")]
        public async Task<IActionResult> ReactLogin([FromBody] LoginRequest request)
        {
            // Validate the user credentials from the database
            var user = await dbContext.Login
                .FirstOrDefaultAsync(l => l.Email == request.userEmail && l.Password == request.password);

            if (user != null)
            {
                var token = GenerateJwtToken(user.Email, user.Type, user.Id);
                return Ok(new { token = token });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(string email, string type, int id)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim("type", type), // Custom claim for type
                new Claim("id", id.ToString()), // Custom claim for ID
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsA32ByteLongKeyForJWT123456")); // Use your actual secret key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: null, // Set to your issuer if you have one
                audience: null, // Set to your audience if you have one
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }

}