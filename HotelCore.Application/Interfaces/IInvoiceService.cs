using System.Collections.Generic;
using System.Threading.Tasks;
using HotelCore.Application.DTOs;

namespace HotelCore.Application.Interfaces
{
    /// <summary>
    /// Giao diện định nghĩa các nghiệp vụ liên quan đến Hóa đơn và Thanh toán.
    /// </summary>
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync();
        Task<CheckoutViewDto?> GetBookingForCheckoutAsync(int bookingId);
        Task<bool> CreateInvoiceAsync(InvoiceCreateDto invoice);
    }
}
