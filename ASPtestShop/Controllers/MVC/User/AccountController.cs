using ASPtestShop.Auth;
using ASPtestShop.Data;
using ASPtestShop.Models.ViewModels.Auth;
using ASPtestShop.Services.Interfaces;
using ASPtestShop.Services.Interfaces.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;

namespace ASPtestShop.Controllers.MVC
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly IUserAuthService _userAuthService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(IUserAuthService userAuthService, 
               UserManager<ApplicationUser> userManager,
               IWebHostEnvironment webHostEnvironment)
        {
            _userAuthService = userAuthService;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /account/login
        [HttpGet("login")]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /account/login
        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userAuthService.LoginAsync(model);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            var displayName = !string.IsNullOrWhiteSpace(result.FullName)
                ? result.FullName
                : result.UserName;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.UserId),
                new Claim(ClaimTypes.Name, displayName),
                new Claim(ClaimTypes.Email, result.Email),
                new Claim(ClaimTypes.Role, "Customer")
            };

            var identity = new ClaimsIdentity(claims, UserCookieAuth.Scheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(7)
                    : DateTimeOffset.UtcNow.AddHours(6)
            };

            await HttpContext.SignInAsync(
                UserCookieAuth.Scheme,
                principal,
                authProperties
            );

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        // POST: /account/logout
        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(UserCookieAuth.Scheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: /account/logout
        [HttpGet("logout")]
        public async Task<IActionResult> LogoutGet()
        {
            await HttpContext.SignOutAsync(UserCookieAuth.Scheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: /account/access-denied
        [HttpGet("access-denied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: /account/register
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /account/register
        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userAuthService.RegisterAsync(model);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;

            return RedirectToAction("Login");
        }
        // GET: /account/profile
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("Không tìm thấy thông tin người dùng");

            var model = new UserProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                AvatarUrl = user.AvatarUrl
            };

            return View(model);
        }
        // POST: /account/update-profile
        [HttpPost("update-profile")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound("Không tìm thấy người dùng!");

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;

            user.Email = model.Email;

            if (model.AvatarFile != null && model.AvatarFile.Length > 0)
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", "Avatars");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.AvatarFile.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.AvatarFile.CopyToAsync(fileStream);
                }

                user.AvatarUrl = "/Uploads/Avatars/" + uniqueFileName;
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View("Profile", model);
        }
    }
}