using System.Threading.Tasks;
using HotelCore.Application.DTOs;
using HotelCore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelCore.WebUI.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IServiceService _serviceService;

        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        public async Task<IActionResult> Index(string searchTerm)
        {
            var services = await _serviceService.GetAllServicesAsync(searchTerm);
            ViewBag.SearchTerm = searchTerm;
            return View(services);
        }

        public IActionResult Create()
        {
            return View(new ServiceCreateDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceCreateDto serviceDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _serviceService.CreateServiceAsync(serviceDto);
                if (result)
                {
                    TempData["Success"] = "Thêm dịch vụ thành công!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Lỗi khi thêm dịch vụ.");
            }
            return View(serviceDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var service = await _serviceService.GetServiceByIdAsync(id);
            if (service == null) return NotFound();

            var updateDto = new ServiceUpdateDto
            {
                ServiceId = service.ServiceId,
                ServiceName = service.ServiceName,
                Unit = service.Unit,
                Price = service.Price,
                Status = service.Status
            };
            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceUpdateDto serviceDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _serviceService.UpdateServiceAsync(serviceDto);
                if (result)
                {
                    TempData["Success"] = "Cập nhật dịch vụ thành công!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Lỗi khi cập nhật dịch vụ.");
            }
            return View(serviceDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _serviceService.DeleteServiceAsync(id);
            TempData["Success"] = "Đã xóa dịch vụ thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
