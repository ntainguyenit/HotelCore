using System.Threading.Tasks;
using HotelCore.Application.DTOs;
using HotelCore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelCore.WebUI.Controllers
{
    [Authorize(Roles = "Quản lý,Lễ tân,Kế toán")]
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        public async Task<IActionResult> Index(string searchTerm = "", int pageNumber = 1)
        {
            int pageSize = 10;
            var invoices = await _invoiceService.GetPagedInvoicesAsync(searchTerm, pageNumber, pageSize);
            ViewBag.SearchTerm = searchTerm;
            return View(invoices);
        }

        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null) return NotFound();
            return View(invoice);
        }

        [HttpGet]
        public async Task<IActionResult> Checkout(int bookingId)
        {
            var checkoutData = await _invoiceService.GetBookingForCheckoutAsync(bookingId);
            if (checkoutData == null) return NotFound();
            return View(checkoutData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoiceCreateDto invoice)
        {
            if (ModelState.IsValid)
            {
                var result = await _invoiceService.CreateInvoiceAsync(invoice);
                if (result)
                {
                    TempData["Success"] = "Thanh toán thành công!";
                    return RedirectToAction(nameof(Index));
                }
                TempData["Error"] = "Lỗi khi xử lý thanh toán. Vui lòng kiểm tra lại.";
            }
            else
            {
                TempData["Error"] = "Dữ liệu thanh toán không hợp lệ.";
            }
            return RedirectToAction(nameof(Checkout), new { bookingId = invoice.BookingId });
        }
    }
}
