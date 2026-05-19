namespace HotelCore.Domain.Enums
{
    /// <summary>
    /// Các phân hạng thành viên của khách hàng thân thiết.
    /// Quyết định mức độ ưu đãi và tích lũy điểm thưởng.
    /// </summary>
    public enum LoyaltyTier
    {
        /// <summary>
        /// Thành viên hạng Đồng (Mặc định khi đăng ký)
        /// </summary>
        Silver,

        /// <summary>
        /// Thành viên hạng Vàng (Tích lũy từ 5,000 điểm)
        /// </summary>
        Gold,

        /// <summary>
        /// Thành viên hạng Kim Cương (Tích lũy từ 15,000 điểm)
        /// </summary>
        Platinum
    }
}
