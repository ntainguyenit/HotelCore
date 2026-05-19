using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelCore.Application.Services;
using HotelCore.Domain.Entities;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;
using HotelCore.Domain.ValueObjects;
using Xunit;

namespace HotelCore.Tests.Integration
{
    /// <summary>
    /// Kiểm thử tích hợp chuỗi nghiệp vụ (Integration Tests).
    /// Giả lập đầy đủ quy trình thực tế từ Đăng ký Khách hàng -> Đặt phòng -> Sử dụng dịch vụ -> 
    /// Tính toán khuyến mãi tối ưu -> Xuất Hóa đơn -> Thanh toán và cập nhật Hạng thành viên.
    /// </summary>
    public class MockDbTransactionTests
    {
        private readonly BookingValidationService _validationService;
        private readonly PromotionEngine _promotionEngine;
        private readonly LoyaltyService _loyaltyService;

        public MockDbTransactionTests()
        {
            _validationService = new BookingValidationService();
            _promotionEngine = new PromotionEngine();
            _loyaltyService = new LoyaltyService();
        }

        [Fact]
        public async Task EndToEnd_GuestFlow_ShouldSucceedWithAccurateCalculations()
        {
            // ==========================================
            // GIAI ĐOẠN 1: KHỞI TẠO DỮ LIỆU BAN ĐẦU
            // ==========================================
            
            // 1. Tạo khách hàng mới (Hạng Silver mặc định, 0 điểm)
            var customer = new Customer("Nguyễn Ngọc Thanh Tài", "tai@gmail.com", "0901234567", "312345678") { CustomerId = 1 };
            Assert.Equal(LoyaltyTier.Silver, customer.Tier);
            Assert.Equal(0, customer.LoyaltyPoints);

            // 2. Tạo phòng cao cấp (Vip Room 302, Tầng 3, giá 2,000,000đ/ngày)
            var room = new Room("302", 2, 2000000m, 3) { RoomId = 302 };
            Assert.Equal(RoomStatus.Available, room.Status);

            // Giả lập danh sách đặt phòng đang hoạt động trong hệ thống
            var activeBookingsList = new List<Booking>();

            // ==========================================
            // GIAI ĐOẠN 2: THỰC HIỆN ĐẶT PHÒNG (BOOKING)
            // ==========================================

            // 3. Đăng ký ở 6 ngày (Đủ điều kiện nhận 8% Long Stay discount)
            var today = DateTime.Today;
            var stayPeriod = new DateRange(today, today.AddDays(6));

            // Xác thực yêu cầu đặt phòng (Validate Booking)
            await _validationService.ValidateBookingRequestAsync(customer, room, stayPeriod, activeBookingsList);

            // Tạo bản ghi đặt phòng mới
            var booking = new Booking(customer.CustomerId, room.RoomId, stayPeriod, room.BasePrice)
            {
                BookingId = 888,
                Customer = customer,
                Room = room
            };

            // Đổi trạng thái phòng thành Occupied
            room.Occupy();
            Assert.Equal(RoomStatus.Occupied, room.Status);

            // Đưa khách nhận phòng thực tế (Check-in)
            booking.CheckIn();
            Assert.Equal(BookingStatus.CheckedIn, booking.Status);

            // ==========================================
            // GIAI ĐOẠN 3: SỬ DỤNG DỊCH VỤ GIA TĂNG (SERVICES)
            // ==========================================

            // Khách hàng gọi món ăn tại phòng (Room Service) và đi Massage tại Spa
            booking.AddServiceUse(1, "Ăn sáng Buffet", 150000m, 2); // 300,000đ
            booking.AddServiceUse(2, "Dịch vụ Massage Thảo dược", 600000m, 1); // 600,000đ
            booking.AddServiceUse(1, "Ăn sáng Buffet", 150000m, 1); // Gọi thêm 1 suất -> tổng 3 suất = 450,000đ

            // Kiểm tra tổng chi phí dịch vụ
            Assert.Equal(2, booking.ServicesUsed.Count);

            // ==========================================
            // GIAI ĐOẠN 4: TÍNH TOÁN CHIẾT KHẤU & XUẤT HÓA ĐƠN
            // ==========================================

            // 4. Áp dụng mã khuyến mãi đặc biệt "SUMMERVIBES" (Giảm giá 20%)
            // Chiết khấu dự kiến: Long Stay (8%) + Silver Member (1%) + Promo Code (20%) = 29% (0.29)
            decimal discountRate = await _promotionEngine.CalculateDiscountRateAsync(booking, "SUMMERVIBES");
            Assert.Equal(0.29m, discountRate);

            // 5. Khởi tạo hóa đơn chi tiết
            var invoice = new Invoice(booking, discountRate)
            {
                InvoiceId = 9999
            };

            // Kiểm tra các phép toán tài chính trên hóa đơn:
            // - Tiền phòng gốc: 6 ngày x 2,000,000 = 12,000,000đ
            // - Tiền dịch vụ gốc: (150K x 3) + (600K x 1) = 1,050,000đ
            // - Tổng doanh thu gốc: 12,000,000 + 1,050,000 = 13,050,000đ
            // - Chiết khấu: 13,050,000đ x 29% = 3,784,500đ
            // - Trước thuế: 13,050,000 - 3,784,500 = 9,265,500đ
            // - Thuế VAT (10%): 9,265,500 x 10% = 926,550đ
            // - Tổng tiền thanh toán cuối cùng: 9,265,500 + 926,550 = 10,192,050đ
            
            Assert.Equal(12000000m, invoice.RoomCharges);
            Assert.Equal(1050000m, invoice.ServiceCharges);
            Assert.Equal(3784500m, invoice.DiscountAmount);
            Assert.Equal(10192050m, invoice.TotalAmount);

            // ==========================================
            // GIAI ĐOẠN 5: THANH TOÁN & QUY ĐỔI LOYALTY
            // ==========================================

            // 6. Thanh toán hóa đơn (Checkout thực tế)
            invoice.MarkAsPaid("Credit Card");
            Assert.True(invoice.IsPaid);
            Assert.Equal(BookingStatus.Completed, booking.Status);

            // Dọn dẹp phòng để chuẩn bị đón khách tiếp theo
            room.ReleaseForCleaning();
            Assert.Equal(RoomStatus.Cleaning, room.Status);
            room.CompleteCleaning();
            Assert.Equal(RoomStatus.Available, room.Status);

            // 7. Xử lý tích lũy điểm thưởng khách hàng thân thiết:
            // - Tiền phòng: 12,000,000đ / 20,000đ = 600 điểm
            // - Dịch vụ: 1,050,000đ / 10,000đ = 105 điểm
            // - Hạng Silver: Hệ số nhân 1.0x
            // - Tổng điểm cộng thêm: 600 + 105 = 705 điểm
            int pointsEarned = await _loyaltyService.CalculatePointsEarnedAsync(invoice);
            Assert.Equal(705, pointsEarned);

            // Tích lũy điểm vào tài khoản khách hàng
            bool isPointsAdded = await _loyaltyService.ProcessLoyaltyPointsForInvoiceAsync(customer, invoice);
            Assert.True(isPointsAdded);
            Assert.Equal(705, customer.LoyaltyPoints);

            // Đang có 705 điểm, hạng thành viên vẫn ở mức Silver (chưa đạt 5000 để lên Gold)
            Assert.Equal(LoyaltyTier.Silver, customer.Tier);
        }
    }
}
