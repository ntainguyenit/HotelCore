using System;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Domain.Entities
{
    /// <summary>
    /// Thực thể đại diện cho các chương trình khuyến mãi/giảm giá của khách sạn.
    /// </summary>
    public class Promotion
    {
        public int PromotionId { get; set; }
        public string PromoCode { get; set; }
        public string Title { get; set; }
        public decimal DiscountPercent { get; set; } // Ví dụ: 0.15 tương đương 15%
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MinimumDaysRequired { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        public Promotion() { }

        public Promotion(string promoCode, string title, decimal discountPercent, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrWhiteSpace(promoCode))
                throw new HotelDomainException("Mã khuyến mãi không được trống.");
            if (discountPercent < 0 || discountPercent > 1.0m)
                throw new HotelDomainException("Tỷ lệ giảm giá phải nằm từ 0.0 đến 1.0.");
            if (endDate <= startDate)
                throw new HotelDomainException("Ngày kết thúc khuyến mãi phải sau ngày bắt đầu.");

            PromoCode = promoCode.ToUpper();
            Title = title;
            DiscountPercent = discountPercent;
            StartDate = startDate;
            EndDate = endDate;
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của mã khuyến mãi tại thời điểm đặt phòng.
        /// </summary>
        public bool IsValidForStay(int durationInDays)
        {
            var today = DateTime.Today;
            if (!IsActive) return false;
            if (today < StartDate.Date || today > EndDate.Date) return false;
            if (durationInDays < MinimumDaysRequired) return false;

            return true;
        }
    }
}
