using System.Threading.Tasks;
using HotelCore.Application.DTOs;
using HotelCore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelCore.WebUI.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return View(employees);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareRoleDropdown();
            return View(new EmployeeCreateDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeCreateDto employee)
        {
            if (ModelState.IsValid)
            {
                if (await _employeeService.CreateEmployeeAsync(employee))
                {
                    TempData["Success"] = "Thêm nhân viên thành công!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Lỗi khi lưu dữ liệu.");
            }
            await PrepareRoleDropdown();
            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null) return NotFound();
            await PrepareRoleDropdown();
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeUpdateDto employee)
        {
            if (ModelState.IsValid)
            {
                if (await _employeeService.UpdateEmployeeAsync(employee))
                {
                    TempData["Success"] = "Cập nhật nhân viên thành công!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Lỗi khi cập nhật dữ liệu.");
            }
            await PrepareRoleDropdown();
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _employeeService.DeleteEmployeeAsync(id);
            TempData["Success"] = "Đã xóa nhân viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        private async Task PrepareRoleDropdown()
        {
            var roles = await _employeeService.GetRolesAsync();
            ViewBag.Roles = new SelectList(roles, "RoleId", "RoleName");
        }
    }
}
