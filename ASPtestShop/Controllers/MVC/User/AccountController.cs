using ASPtestShop.Auth;
using ASPtestShop.Models.ViewModels.Auth;
using ASPtestShop.Services.Interfaces.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASPtestShop.Controllers.MVC
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly IUserAuthService _userAuthService;

        public AccountController(IUserAuthService userAuthService)
        {
            _userAuthService = userAuthService;
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

        // GET: /account/forgot-password
        [HttpGet("forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /account/forgot-password
        [HttpPost("forgot-password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userAuthService.ForgotPasswordAsync(model);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(result.ResetToken))
            {
                var resetLink = Url.Action(
                    "ResetPassword",
                    "Account",
                    new
                    {
                        email = result.Email,
                        resetToken = result.ResetToken
                    },
                    Request.Scheme
                );

                TempData["ResetLink"] = resetLink;
            }

            TempData["SuccessMessage"] = result.Message;

            return RedirectToAction("ForgotPassword");
        }

        // GET: /account/reset-password
        [HttpGet("reset-password")]
        public IActionResult ResetPassword(string email, string resetToken)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(resetToken))
            {
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordViewModel
            {
                Email = email,
                ResetToken = resetToken
            };

            return View(model);
        }

        // POST: /account/reset-password
        [HttpPost("reset-password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userAuthService.ResetPasswordAsync(model);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;

            return RedirectToAction("Login");
        }
    }
}