using System.Threading.Tasks;
using HotelCore.Domain.Entities;

namespace HotelCore.Application.Interfaces
{
    /// <summary>
    /// Giao diện xử lý tính toán các chương trình ưu đãi, khuyến mãi động.
    /// </summary>
    public interface IPromotionEngine
    {
        /// <summary>
        /// Tính toán tỷ lệ giảm giá tối ưu nhất cho khách hàng dựa trên Booking, hạng thành viên, mã Promo.
        /// </summary>
        Task<decimal> CalculateDiscountRateAsync(Booking booking, string promoCode);
    }
}
