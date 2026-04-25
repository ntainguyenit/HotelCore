using System.Threading.Tasks;
using HotelCore.Application.DTOs;
using HotelCore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelCore.WebUI.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            return View(invoices);
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
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Lỗi khi xử lý thanh toán.");
            }
            return RedirectToAction(nameof(Checkout), new { bookingId = invoice.BookingId });
        }
    }
}
