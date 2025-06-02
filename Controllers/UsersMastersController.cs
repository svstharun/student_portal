using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalDBFirst.Models;

using System.Linq;
//using StudentPortalDBFirst.Models.viewmodel;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentPortalDBFirst.Controllers
{
    public class UsersMastersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<UsersMaster> _passwordHasher;

        public UsersMastersController(AppDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<UsersMaster>();
        }

        // GET: Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

          
            if (ModelState.IsValid)
            {
                var user = new UsersMaster
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Username = model.Username,
                    Email = model.Email,
                    Mobile = model.Mobile
                    //PasswordHash = _passwordHasher.HashPassword(user, model.Password)
                    

                };



                // Hash the password

                user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

                // Add user to the database

                _context.UsersMasters.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            // If model validation fails, return the same view with validation messages

            return View(model);
        }

        // GET: Login
        public IActionResult Login()
        {
            // If user is already authenticated, redirect to home
            if (User.Identity.IsAuthenticated)
            {
                    return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Please enter both username and password.";
                return View();
            }

            var user = await _context.UsersMasters.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View();
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Success)
            {
                // Keep session for backward compatibility
                HttpContext.Session.SetInt32("Id", user.Id);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("FullName", $"{user.FirstName} {user.LastName}");

                // Add cookie authentication (this is the key fix)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("FullName", $"{user.FirstName} {user.LastName}"),
                    // Add other claims as needed
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Redirect to Students Home
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Invalid username or password.";
            return View();
        }

        // Logout
        public async Task<IActionResult> Logout()
        {
            // Clear session
            HttpContext.Session.Clear();

            // Sign out of cookie authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

        // GET: Edit Profile
        public IActionResult EditProfile()
        {
            var userId = HttpContext.Session.GetInt32("Id"); // Fixed session key to match what's set in Login
            if (userId == null) return RedirectToAction("Login");

            var user = _context.UsersMasters.Find(userId);
            if (user == null) return RedirectToAction("Login");

            return View(user);
        }

        // POST: Edit Profile                                                      
        [HttpPost]
        [ValidateAntiForgeryToken]                                           
                                                                                
        public async Task<IActionResult> EditProfile(UsersMaster model)      /*@@@model contains updated values of the profile fields
                                                                                     like FirstName, LastName etc.*/
        {
            var userId = HttpContext.Session.GetInt32("Id");         /* Pulls the current logged-in user's ID from session storage..*/
            if (userId == null) return RedirectToAction("Login");     /*If the session has expired or the user is not logged in,
                                                                        it redirects them to the Login page...*/

            if (ModelState.IsValid)        // If all validations pass, the method proceeds to update the database.

            {
                var existingUser = await _context.UsersMasters.FindAsync(userId);   // Fetches the existing user from the database using the ID from session.

                if (existingUser == null) return RedirectToAction("Login");

                // Update only allowed fields
                existingUser.FirstName = model.FirstName;                       /* idhi okka method which accepts the posted form data
                                                                                        as UsersMaster model instance. */
                existingUser.LastName = model.LastName;
                existingUser.Email = model.Email;                           // model ante updated values and existing user ante current user of id.
                existingUser.Mobile = model.Mobile;

                _context.Update(existingUser);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Students");
            }

            // Reload the edit profile view user from DB by fetching original user data from db without saving error data 
            var user = await _context.UsersMasters.FindAsync(userId);
            return View(user);              
        }

        // POST: Change Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) // Checks if the submitted data passes all validation rules defined in your ViewModel.cs >> here lopala (like required fields, password matching, etc.).
            {
                TempData["Error"] = "Please fix the errors wrong password";
                return RedirectToAction("Index", "Students");
            }

            var userId = HttpContext.Session.GetInt32("Id");
            if (userId == null)
            {
                TempData["Error"] = "Session expired. Please log in again.";
                return RedirectToAction("Login");
            }

            var user = await _context.UsersMasters.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                TempData["Error"] = "User not found mawa.";
                return RedirectToAction("Login");
            }

            // Check old password
            var passwordCheck = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.OldPassword);
            if (passwordCheck != PasswordVerificationResult.Success)
            {
                TempData["Error"] = "Old password is incorrect.";
                return RedirectToAction("Index", "Students");
            }

            // If old and new passwords are the same, reject it (optional but recommended)
            var newPasswordCheck = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.NewPassword);
            if (newPasswordCheck == PasswordVerificationResult.Success)
            {
                TempData["Error"] = "New password cannot be the same as the old password.";
                return RedirectToAction("Index", "Students");
            }

            // Update with new password
            user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
            _context.UsersMasters.Update(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Password changed successfully. Please log in again.";

            // Clear session and logout
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

    }
}