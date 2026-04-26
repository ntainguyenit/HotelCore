using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using HotelCore.Application.DTOs;
using HotelCore.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelCore.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginDto());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _authService.LoginAsync(loginDto.Username, loginDto.Password);
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.EmployeeId.ToString()),
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Role, user.RoleName),
                        new Claim("Username", user.Username),
                        new Claim("HotelName", user.HotelName)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = loginDto.RememberMe
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không chính xác.");
            }
            return View(loginDto);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            int employeeId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var profile = await _authService.GetProfileAsync(employeeId);
            return View(profile);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserProfileDto profileDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.UpdateProfileAsync(profileDto);
                if (result)
                {
                    TempData["Success"] = "Cập nhật hồ sơ thành công!";
                    return RedirectToAction(nameof(Profile));
                }
                ModelState.AddModelError("", "Lỗi khi cập nhật hồ sơ.");
            }
            return View("Profile", profileDto);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            int employeeId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _authService.ChangePasswordAsync(employeeId, currentPassword, newPassword);
            if (result)
            {
                TempData["Success"] = "Đổi mật khẩu thành công!";
            }
            else
            {
                TempData["Error"] = "Mật khẩu hiện tại không chính xác.";
            }
            return RedirectToAction(nameof(Profile));
        }

        // Action phụ để khởi tạo tài khoản Admin nếu chưa có
        [AllowAnonymous]
        public async Task<IActionResult> Seed()
        {
            // Tạo tài khoản cho nhân viên ID 1 (Nguyễn Văn A)
            await _authService.CreateInitialAccountAsync(1, "admin", "admin123");
            return Content("Seed completed: admin / admin123");
        }
    }
}
