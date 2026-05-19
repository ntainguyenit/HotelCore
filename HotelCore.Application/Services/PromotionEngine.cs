using System;
using System.Threading.Tasks;
using HotelCore.Application.Interfaces;
using HotelCore.Domain.Entities;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Services
{
    /// <summary>
    /// Engine tính toán khuyến mãi phức tạp và chuyên nghiệp.
    /// Kết hợp nhiều nguồn ưu đãi: hạng thành viên, độ dài ngày ở, và mã khuyến mãi cụ thể.
    /// </summary>
    public class PromotionEngine : IPromotionEngine
    {
        public async Task<decimal> CalculateDiscountRateAsync(Booking booking, string promoCode)
        {
            if (booking == null) return 0;

            decimal totalDiscountRate = 0;

            // 1. Khuyến mãi theo độ dài lưu trú (Long stay discount)
            var duration = booking.DateRange.DurationInDays;
            if (duration >= 10)
            {
                totalDiscountRate += 0.15m; // 15% discount
            }
            else if (duration >= 5)
            {
                totalDiscountRate += 0.08m; // 8% discount
            }
            else if (duration >= 3)
            {
                totalDiscountRate += 0.03m; // 3% discount
            }

            // 2. Ưu đãi theo hạng thành viên của khách hàng
            if (booking.Customer != null)
            {
                switch (booking.Customer.Tier)
                {
                    case LoyaltyTier.Platinum:
                        totalDiscountRate += 0.12m; // Platinum giảm tiếp 12%
                        break;
                    case LoyaltyTier.Gold:
                        totalDiscountRate += 0.06m; // Gold giảm tiếp 6%
                        break;
                    case LoyaltyTier.Silver:
                    default:
                        totalDiscountRate += 0.01m; // Silver giảm tiếp 1% tri ân
                        break;
                }
            }

            // 3. Ưu đãi theo mã khuyến mãi đặc biệt (Promo code)
            if (!string.IsNullOrWhiteSpace(promoCode))
            {
                var cleanCode = promoCode.Trim().ToUpper();
                if (cleanCode == "WELCOME10")
                {
                    totalDiscountRate += 0.10m;
                }
                else if (cleanCode == "SUMMERVIBES")
                {
                    totalDiscountRate += 0.20m;
                }
                else if (cleanCode == "PLATINUMONLY" && booking.Customer?.Tier == LoyaltyTier.Platinum)
                {
                    totalDiscountRate += 0.25m;
                }
            }

            // Khống chế tỷ lệ giảm giá tối đa để đảm bảo doanh thu cho khách sạn
            // Tổng giảm giá tối đa không được vượt quá 35% tổng hóa đơn
            const decimal MaxAllowedDiscountRate = 0.35m;
            if (totalDiscountRate > MaxAllowedDiscountRate)
            {
                totalDiscountRate = MaxAllowedDiscountRate;
            }

            return await Task.FromResult(Math.Round(totalDiscountRate, 2));
        }
    }
}
