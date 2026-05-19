using System;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Domain.Entities
{
    /// <summary>
    /// Thực thể đại diện cho Khách hàng (Guest).
    /// Quản lý thông tin liên hệ, điểm thưởng và thăng hạng thành viên.
    /// </summary>
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string IdentityCard { get; set; }
        public int LoyaltyPoints { get; private set; }
        public LoyaltyTier Tier { get; private set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Customer()
        {
            Tier = LoyaltyTier.Silver;
            LoyaltyPoints = 0;
        }

        public Customer(string fullName, string email, string phone, string identityCard) : this()
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new HotelDomainException("Tên khách hàng không được để trống.");
            
            if (string.IsNullOrWhiteSpace(phone))
                throw new HotelDomainException("Số điện thoại không được để trống.");

            FullName = fullName;
            Email = email;
            Phone = phone;
            IdentityCard = identityCard;
        }

        /// <summary>
        /// Cộng điểm thưởng tích lũy cho khách hàng sau khi hoàn tất thanh toán hóa đơn.
        /// Tự động nâng cấp phân hạng thành viên dựa trên số điểm tích lũy.
        /// </summary>
        public void AddPoints(int points)
        {
            if (points < 0)
                throw new HotelDomainException("Điểm thưởng cộng thêm không thể là số âm.");

            LoyaltyPoints += points;
            UpdateTier();
        }

        /// <summary>
        /// Quy đổi điểm thưởng để giảm giá.
        /// </summary>
        public void RedeemPoints(int pointsToRedeem)
        {
            if (pointsToRedeem < 0)
                throw new HotelDomainException("Số điểm quy đổi không thể âm.");
            if (pointsToRedeem > LoyaltyPoints)
                throw new HotelDomainException("Khách hàng không đủ điểm tích lũy để thực hiện quy đổi.");

            LoyaltyPoints -= pointsToRedeem;
            UpdateTier();
        }

        /// <summary>
        /// Kiểm tra và nâng cấp hạng dựa trên điểm tích lũy hiện tại.
        /// </summary>
        private void UpdateTier()
        {
            if (LoyaltyPoints >= 15000)
            {
                Tier = LoyaltyTier.Platinum;
            }
            else if (LoyaltyPoints >= 5000)
            {
                Tier = LoyaltyTier.Gold;
            }
            else
            {
                Tier = LoyaltyTier.Silver;
            }
        }
    }
}
