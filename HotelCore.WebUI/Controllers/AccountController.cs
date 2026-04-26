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
        public IActionResult AccessDenied()
        {
            return View();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("", "Vui lòng nhập Tên đăng nhập.");
                return View();
            }

            var result = await _authService.CreatePasswordResetRequestAsync(username);
            if (result)
            {
                ViewBag.SuccessMessage = "Yêu cầu khôi phục đã được gửi tới Quản trị viên. Vui lòng liên hệ Admin để nhận mật khẩu mới.";
                return View();
            }
            
            ModelState.AddModelError("", "Tên đăng nhập không tồn tại trong hệ thống.");
            return View();
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

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordDto());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (!ModelState.IsValid) return View(model);

            int employeeId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _authService.ChangePasswordAsync(employeeId, model.CurrentPassword, model.NewPassword);
            if (result)
            {
                TempData["Success"] = "Đổi mật khẩu thành công!";
                return RedirectToAction(nameof(Profile));
            }
            
            ModelState.AddModelError("CurrentPassword", "Mật khẩu hiện tại không chính xác.");
            return View(model);
        }

        // Action phụ để khởi tạo tài khoản Admin và các vai trò khác
        [AllowAnonymous]
        public async Task<IActionResult> Seed()
        {
            // 1. Admin (Quản lý)
            await _authService.CreateInitialAccountAsync(1, "admin", "admin");

            // 2. Lễ tân (Dựa trên RoleId 2)
            await _authService.CreateInitialAccountAsync(94, "letan", "letan");

            // 3. Buồng phòng (RoleId 4)
            await _authService.CreateInitialAccountAsync(99, "buongphong", "buongphong");

            // 4. Kinh doanh (RoleId 7)
            await _authService.CreateInitialAccountAsync(121, "kinhdoanh", "kinhdoanh");

            // 5. Kế toán (RoleId 3)
            await _authService.CreateInitialAccountAsync(3, "ketoan", "ketoan");

            return Content("Seed completed: admin/admin, letan/letan, buongphong/buongphong, kinhdoanh/kinhdoanh, ketoan/ketoan");
        }
    }
}
