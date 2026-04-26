using System.Threading.Tasks;
using HotelCore.Application.DTOs;
using HotelCore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelCore.WebUI.Controllers
{
    /// <summary>
    /// Controller quản lý thông tin khách hàng.
    /// </summary>
    [Authorize(Roles = "Quản lý,Lễ tân,Kinh doanh")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Hiển thị danh sách khách hàng.
        /// </summary>
        public async Task<IActionResult> Index(string searchTerm = "", int pageNumber = 1)
        {
            int pageSize = 10;
            var customers = await _customerService.GetPagedCustomersAsync(searchTerm, pageNumber, pageSize);
            ViewBag.SearchTerm = searchTerm;
            return View(customers);
        }

        /// <summary>
        /// Hiển thị form tạo mới khách hàng.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CustomerCreateDto());
        }

        /// <summary>
        /// Xử lý lưu khách hàng mới.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerCreateDto customer)
        {
            if (ModelState.IsValid)
            {
                var result = await _customerService.CreateCustomerAsync(customer);
                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi lưu dữ liệu.");
            }
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAjax([FromBody] CustomerCreateDto customer)
        {
            if (ModelState.IsValid)
            {
                var result = await _customerService.CreateCustomerAsync(customer);
                if (result)
                {
                    // Lấy lại danh sách khách hàng mới nhất (hoặc chỉ cần trả về OK)
                    // Ở đây ta có thể trả về thông tin khách hàng vừa tạo nếu Service hỗ trợ trả về ID
                    // Tạm thời trả về thành công
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        }

        /// <summary>
        /// Hiển thị form cập nhật khách hàng.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var updateDto = new CustomerUpdateDto
            {
                CustomerId = customer.CustomerId,
                FullName = customer.FullName,
                Gender = customer.Gender,
                DOB = customer.DOB,
                IdCardNumber = customer.IdCardNumber,
                Address = customer.Address,
                Phone = customer.Phone,
                Email = customer.Email
            };

            return View(updateDto);
        }

        /// <summary>
        /// Xử lý cập nhật thông tin khách hàng.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerUpdateDto customer)
        {
            if (ModelState.IsValid)
            {
                var result = await _customerService.UpdateCustomerAsync(customer);
                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật dữ liệu.");
            }
            return View(customer);
        }

        /// <summary>
        /// Xử lý xóa khách hàng.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            if (!result)
            {
                // Có thể thêm TempData để thông báo lỗi nếu không xóa được do ràng buộc
                return BadRequest("Không thể xóa khách hàng này.");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
