namespace HotelCore.Domain.Enums
{
    /// <summary>
    /// Vòng đời trạng thái của một lượt đặt phòng.
    /// </summary>
    public enum BookingStatus
    {
        /// <summary>
        /// Đã xác nhận đặt phòng thành công
        /// </summary>
        Confirmed,

        /// <summary>
        /// Khách hàng đã nhận phòng và đang lưu trú
        /// </summary>
        CheckedIn,

        /// <summary>
        /// Khách hàng đã thanh toán và trả phòng thành công
        /// </summary>
        Completed,

        /// <summary>
        /// Lượt đặt phòng bị hủy bỏ
        /// </summary>
        Cancelled
    }
}
