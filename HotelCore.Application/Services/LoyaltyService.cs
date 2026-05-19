using System;
using System.Threading.Tasks;
using HotelCore.Application.Interfaces;
using HotelCore.Domain.Entities;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Services
{
    /// <summary>
    /// Triển khai dịch vụ quản lý điểm thưởng khách hàng thân thiết.
    /// Tính toán thưởng dựa trên cơ cấu doanh thu phòng và dịch vụ gia tăng.
    /// </summary>
    public class LoyaltyService : ILoyaltyService
    {
        public async Task<int> CalculatePointsEarnedAsync(Invoice invoice)
        {
            if (invoice == null || !invoice.IsPaid) return 0;

            // 1. Tỷ lệ quy đổi điểm cơ bản:
            // - Mỗi 20,000đ chi trả tiền phòng tích lũy 1 điểm.
            // - Mỗi 10,000đ chi trả cho dịch vụ (F&B, Spa, giặt ủi...) tích lũy 1 điểm (để khuyến khích dùng dịch vụ).
            decimal roomCostPerPoint = 20000m;
            decimal serviceCostPerPoint = 10000m;

            int pointsFromRoom = (int)Math.Floor(invoice.RoomCharges / roomCostPerPoint);
            int pointsFromServices = (int)Math.Floor(invoice.ServiceCharges / serviceCostPerPoint);

            double basePoints = pointsFromRoom + pointsFromServices;

            // 2. Hệ số nhân điểm thưởng dựa trên hạng thành viên hiện tại của khách hàng tại thời điểm xuất hóa đơn
            double multiplier = 1.0;
            if (invoice.Booking?.Customer != null)
            {
                switch (invoice.Booking.Customer.Tier)
                {
                    case LoyaltyTier.Platinum:
                        multiplier = 1.5; // Platinum được nhân 1.5 lần điểm tích lũy
                        break;
                    case LoyaltyTier.Gold:
                        multiplier = 1.2; // Gold được nhân 1.2 lần điểm tích lũy
                        break;
                    case LoyaltyTier.Silver:
                    default:
                        multiplier = 1.0;
                        break;
                }
            }

            int finalPoints = (int)Math.Round(basePoints * multiplier);
            return await Task.FromResult(finalPoints);
        }

        public async Task<bool> ProcessLoyaltyPointsForInvoiceAsync(Customer customer, Invoice invoice)
        {
            if (customer == null || invoice == null) return false;
            if (!invoice.IsPaid) return false;

            int points = await CalculatePointsEarnedAsync(invoice);
            if (points > 0)
            {
                var oldTier = customer.Tier;
                customer.AddPoints(points);

                // Ghi nhận log thăng hạng thành viên
                if (customer.Tier != oldTier)
                {
                    Console.WriteLine($"[LOYALTY ALERT] Khách hàng {customer.FullName} đã thăng hạng từ {oldTier} lên {customer.Tier}!");
                }
                return true;
            }

            return false;
        }
    }
}
