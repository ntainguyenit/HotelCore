using System.Threading.Tasks;
using HotelCore.Domain.Entities;

namespace HotelCore.Application.Interfaces
{
    /// <summary>
    /// Dịch vụ quản lý điểm thưởng và phân hạng Khách hàng thân thiết (Loyalty Program).
    /// </summary>
    public interface ILoyaltyService
    {
        /// <summary>
        /// Tính toán số điểm tích lũy được dựa trên hóa đơn thanh toán thực tế.
        /// </summary>
        Task<int> CalculatePointsEarnedAsync(Invoice invoice);

        /// <summary>
        /// Tự động cập nhật điểm thưởng cho Khách hàng và thông báo thăng hạng nếu đạt điều kiện.
        /// </summary>
        Task<bool> ProcessLoyaltyPointsForInvoiceAsync(Customer customer, Invoice invoice);
    }
}
