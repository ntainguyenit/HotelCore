using System.Threading.Tasks;
using HotelCore.Application.DTOs;
using HotelCore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelCore.WebUI.Controllers
{
    /// <summary>
    /// Controller quản lý nghiệp vụ Đặt phòng.
    /// </summary>
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Quản lý,Lễ tân")]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Hiển thị giao diện Form Đặt phòng mới.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareDropdownDataAsync();
            return View(new BookingRequestDto());
        }

        /// <summary>
        /// Nhận dữ liệu từ Form gửi lên và xử lý lưu vào Database.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingRequestDto request)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra logic ngày tháng cơ bản
                if (request.CheckOutDate <= request.CheckInDate)
                {
                    ModelState.AddModelError("CheckOutDate", "Ngày trả phòng phải sau ngày nhận phòng.");
                    await PrepareDropdownDataAsync();
                    return View(request);
                }

                // Gọi hàm tạo Booking (có chứa SqlTransaction)
                bool isSuccess = await _bookingService.CreateBookingAsync(request);

                if (isSuccess)
                {
                    // Nếu thành công, chuyển hướng về trang chủ Dashboard
                    // (Lưu ý: trong thực tế có thể hiển thị thông báo TempData["SuccessMessage"])
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi trong quá trình đặt phòng. Vui lòng thử lại.");
                }
            }

            // Nếu form có lỗi Validate hoặc lỗi DB, hiển thị lại trang Create kèm lỗi
            await PrepareDropdownDataAsync();
            return View(request);
        }

        /// <summary>
        /// Hàm hỗ trợ chuẩn bị dữ liệu cho Dropdown List (Select boxes).
        /// </summary>
        private async Task PrepareDropdownDataAsync()
        {
            var customers = await _bookingService.GetCustomersAsync();
            var availableRooms = await _bookingService.GetAvailableRoomsAsync();

            ViewBag.CustomerList = new SelectList(customers, "CustomerId", "DisplayName");
            ViewBag.RoomList = new SelectList(availableRooms, "RoomId", "DisplayName");
        }
    }
}
