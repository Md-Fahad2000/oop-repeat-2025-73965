using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using workshopManagementSystem.Razor;
using workshopManagementSystem.Razor.Models;
using workshopManagementSystem.Domain;
using ErrorViewModel = workshopManagementSystem.Domain.ErrorViewModel;

namespace workshopManagementSystem.Razor.Controllers
{
    public class HomeController : BaseController
    {
        private readonly MyDbContext _dataContext;

        public HomeController(MyDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IActionResult DisplayMainPage()
        {
            var currentUserSession = HttpContext.Session.GetString("UserSession");
            
            if (!string.IsNullOrEmpty(currentUserSession))
            {
                return NavigateToUserDashboard();
            }
            
            return View("ShowHomePage");
        }

        public IActionResult DisplayAuthenticationForm()
        {
            var existingSession = HttpContext.Session.GetString("UserSession");
            
            if (!string.IsNullOrEmpty(existingSession))
            {
                return NavigateToUserDashboard();
            }
            
            return View("ShowLoginPage");
        }

        [HttpPost]
        public async Task<IActionResult> AuthenticateUserCredentials(Customer loginRequest)
        {
            const string systemAdminEmail = "admin@carservice.com";
            const string systemPassword = "Dorset001^";
            const string firstTechnicianEmail = "mechanic1@carservice.com";
            const string secondTechnicianEmail = "mechanic2@carservice.com";

            await InitializeSystemUsers();

            var authenticatedCustomer = await ValidateCustomerCredentials(loginRequest);

            // Handle technician authentication
            if (IsTechnicianLogin(loginRequest, firstTechnicianEmail, secondTechnicianEmail, systemPassword))
            {
                return await ProcessTechnicianAuthentication(loginRequest);
            }

            // Handle administrator authentication
            if (IsAdministratorLogin(loginRequest, systemAdminEmail, systemPassword))
            {
                return ProcessAdministratorAuthentication();
            }

            // Handle customer authentication
            if (authenticatedCustomer != null)
            {
                return ProcessCustomerAuthentication(authenticatedCustomer);
            }

            ViewBag.ErrorMessage = "Invalid email address or password provided.";
            return View("ShowLoginPage");
        }

        public IActionResult DisplayRegistrationForm()
        {
            return RedirectToAction("DisplayAuthenticationForm");
        }

        public IActionResult NavigateToUserDashboard()
        {
            var userSessionId = HttpContext.Session.GetString("UserSession");
            
            if (string.IsNullOrEmpty(userSessionId))
            {
                return RedirectToAction("DisplayAuthenticationForm");
            }

            ViewData["MySession"] = userSessionId;
            var userAccessLevel = HttpContext.Session.GetString("UserRole");

            return DetermineUserDestination(userAccessLevel);
        }

        public IActionResult TerminateUserSession()
        {
            var activeSession = HttpContext.Session.GetString("UserSession");
            
            if (!string.IsNullOrEmpty(activeSession))
            {
                ClearUserSessionData();
                return RedirectToAction("DisplayAuthenticationForm");
            }
            
            return RedirectToAction("DisplayAuthenticationForm");
        }

        public IActionResult DisplayPrivacyInformation()
        {
            return View("ShowPrivacyPage");
        }



        private async Task InitializeSystemUsers()
        {
            await CreateTechnicianAccounts();
            await CreateAdministratorAccount();
            await CreateCustomerAccounts();
            await _dataContext.SaveChangesAsync();
        }

        private async Task CreateTechnicianAccounts()
        {
            var technicianAccounts = new[]
            {
                new { Email = "mechanic1@carservice.com", Name = "Mechanic 1" },
                new { Email = "mechanic2@carservice.com", Name = "Mechanic 2" }
            };

            foreach (var account in technicianAccounts)
            {
                if (!await _dataContext.Mechanics.AnyAsync(m => m.TechnicianEmailAddress == account.Email))
                {
                    _dataContext.Mechanics.Add(new Mechanic
                    {
                        TechnicianFullName = account.Name,
                        TechnicianEmailAddress = account.Email,
                        TechnicianAccessPassword = "Dorset001^"
                    });
                }
            }
        }

        private async Task CreateAdministratorAccount()
        {
            const string adminEmail = "admin@carservice.com";
            
            if (!await _dataContext.Admins.AnyAsync(a => a.AdministratorEmailAddress == adminEmail))
            {
                _dataContext.Admins.Add(new Admin
                {
                    AdministratorFullName = "System Administrator",
                    AdministratorEmailAddress = adminEmail,
                    AdministratorAccessPassword = "Dorset001^"
                });
            }
        }

        private async Task CreateCustomerAccounts()
        {
            var customerAccounts = new[]
            {
                new { Email = "customer1@carservice.com", Name = "John Smith" },
                new { Email = "customer2@carservice.com", Name = "Jane Doe" }
            };

            foreach (var account in customerAccounts)
            {
                if (!await _dataContext.Customers.AnyAsync(c => c.CustomerEmailAddress == account.Email))
                {
                    _dataContext.Customers.Add(new Customer
                    {
                        CustomerFullName = account.Name,
                        CustomerEmailAddress = account.Email,
                        CustomerAccessPassword = "Dorset001^"
                    });
                }
            }
        }

        private async Task<Customer?> ValidateCustomerCredentials(Customer loginRequest)
        {
            return await _dataContext.Customers
                .FirstOrDefaultAsync(c => 
                    c.CustomerEmailAddress == loginRequest.CustomerEmailAddress && 
                    c.CustomerAccessPassword == loginRequest.CustomerAccessPassword);
        }

        private bool IsTechnicianLogin(Customer loginRequest, string email1, string email2, string password)
        {
            var isTechnicianEmail = loginRequest.CustomerEmailAddress == email1 || 
                                   loginRequest.CustomerEmailAddress == email2;
            var isCorrectPassword = loginRequest.CustomerAccessPassword == password;
            
            return isTechnicianEmail && isCorrectPassword;
        }

        private bool IsAdministratorLogin(Customer loginRequest, string adminEmail, string password)
        {
            return loginRequest.CustomerEmailAddress == adminEmail && 
                   loginRequest.CustomerAccessPassword == password;
        }

        private async Task<IActionResult> ProcessTechnicianAuthentication(Customer loginRequest)
        {
            var technician = await _dataContext.Mechanics
                .FirstOrDefaultAsync(m => 
                    m.TechnicianEmailAddress == loginRequest.CustomerEmailAddress && 
                    m.TechnicianAccessPassword == loginRequest.CustomerAccessPassword);

            if (technician != null)
            {
                EstablishUserSession(technician.TechnicianAccountId.ToString(), "Mechanic");
                return RedirectToAction("ShowMechanicDashboard", "Mechanic");
            }

            return RedirectToAction("DisplayAuthenticationForm");
        }

        private IActionResult ProcessAdministratorAuthentication()
        {
            EstablishUserSession("admin", "Admin");
            return RedirectToAction("ShowCustomerList", "Admin");
        }

        private IActionResult ProcessCustomerAuthentication(Customer customer)
        {
            EstablishUserSession(customer.CustomerAccountId.ToString(), "Customer");
            return RedirectToAction("ShowCustomerDashboard", "Customer");
        }

        private void EstablishUserSession(string sessionId, string userType)
        {
            HttpContext.Session.SetString("UserSession", sessionId);
            HttpContext.Session.SetString("UserRole", userType);
        }

        private IActionResult DetermineUserDestination(string? userAccessLevel)
        {
            return userAccessLevel switch
            {
                "Admin" => RedirectToAction("ShowCustomerList", "Admin"),
                "Mechanic" => RedirectToAction("ShowMechanicDashboard", "Mechanic"),
                "Customer" => RedirectToAction("ShowCustomerDashboard", "Customer"),
                _ => RedirectToAction("DisplayAuthenticationForm")
            };
        }

        private void ClearUserSessionData()
        {
            HttpContext.Session.Remove("UserSession");
            HttpContext.Session.Remove("UserRole");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult DisplayErrorPage()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}