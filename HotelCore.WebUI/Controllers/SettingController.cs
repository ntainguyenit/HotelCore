using System;
using System.IO;
using System.Threading.Tasks;
using HotelCore.Application.DTOs;
using HotelCore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace HotelCore.WebUI.Controllers
{
    [Authorize(Roles = "Quản lý")]
    public class SettingController : Controller
    {
        private readonly ISettingService _settingService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SettingController(ISettingService settingService, IWebHostEnvironment webHostEnvironment)
        {
            _settingService = settingService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var settings = await _settingService.GetSystemSettingsAsync();
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(SystemSettingsDto settingsDto)
        {
            if (ModelState.IsValid)
            {
                if (settingsDto.LogoFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string fileName = "logo.png";
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await settingsDto.LogoFile.CopyToAsync(fileStream);
                    }
                    settingsDto.LogoPath = "/images/" + fileName;
                }

                var result = await _settingService.UpdateSystemSettingsAsync(settingsDto);
                if (result)
                {
                    TempData["Success"] = "Cập nhật cài đặt thành công!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Lỗi khi cập nhật cài đặt.");
            }
            return View("Index", settingsDto);
        }
    }
}
