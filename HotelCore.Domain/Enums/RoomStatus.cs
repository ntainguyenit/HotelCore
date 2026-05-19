namespace HotelCore.Domain.Enums
{
    /// <summary>
    /// Các trạng thái hiện tại của một phòng trong khách sạn.
    /// </summary>
    public enum RoomStatus
    {
        /// <summary>
        /// Phòng đang trống và sẵn sàng đón khách
        /// </summary>
        Available,

        /// <summary>
        /// Phòng đang có khách lưu trú
        /// </summary>
        Occupied,

        /// <summary>
        /// Phòng đang trong quá trình dọn dẹp vệ sinh
        /// </summary>
        Cleaning,

        /// <summary>
        /// Phòng đang bảo trì trang thiết bị
        /// </summary>
        Maintenance
    }
}
