namespace HotelCore.Application.DTOs
{
    /// <summary>
    /// DTO chứa thông tin Khách hàng để hiển thị lên Dropdown list (Select box).
    /// </summary>
    public class CustomerDropdownDto
    {
        /// <summary>
        /// Mã định danh khách hàng.
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Họ tên đầy đủ của khách hàng kèm theo số điện thoại (để dễ nhận diện).
        /// </summary>
        public string DisplayName { get; set; }
    }
}
