namespace HotelCore.Application.DTOs
{
    /// <summary>
    /// DTO chứa thông tin Phòng trống để hiển thị lên Dropdown list.
    /// </summary>
    public class RoomDropdownDto
    {
        /// <summary>
        /// Mã định danh phòng.
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// Tên phòng kèm theo loại phòng và giá (vd: Phòng 101 - Standard - 500,000đ).
        /// </summary>
        public string? DisplayName { get; set; }
    }
}
