using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PantryManagementSystem.Models.DTO;
using PantryManagementSystem.Repositories;
using PantryManagementSystem.Repositories.Interfaces;
using System.Security.Claims;

namespace PantryManagementSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenRepository _tokenRepo;
        private readonly string DefaultRole = "Staff";

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepo)
        {
            _userManager = userManager;
            _tokenRepo = tokenRepo;
        }

        // -----------------------------
        // Register GET
        // -----------------------------
        [HttpGet]
        public IActionResult Register() => View();

        // -----------------------------
        // Register POST
        // -----------------------------
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestDTO dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var identityUser = new IdentityUser { UserName = dto.Username, Email = dto.Username };
            var result = await _userManager.CreateAsync(identityUser, dto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(identityUser, DefaultRole);
                TempData["Message"] = $"✅ Registered successfully! Role: {DefaultRole}";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(dto);
        }

        // -----------------------------
        // Login GET
        // -----------------------------
        [HttpGet]
        public IActionResult Login() => View();

        // -----------------------------
        // Login POST
        // -----------------------------
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                var roles = await _userManager.GetRolesAsync(user);

                // ✅ Add NameIdentifier claim (Identity User Id)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName)
                };

                claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                // JWT token for API usage
                var jwt = _tokenRepo.CreateJWTToken(user, roles.ToList());
                Response.Cookies.Append("JWTToken", jwt, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // Set true in production
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(1)
                });

                TempData["Message"] = $"✅ Logged in! Roles: {string.Join(", ", roles)}";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "❌ Invalid Username or Password");
            return View(dto);
        }

        // -----------------------------
        // Logout
        // -----------------------------
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("JWTToken");
            return RedirectToAction("Login");
        }
    }
}
