using System.Threading.Tasks;
using HotelCore.Application.DTOs;

namespace HotelCore.Application.Interfaces
{
    /// <summary>
    /// Giao diện xử lý xác thực và quản lý tài khoản người dùng
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Xác thực người dùng đăng nhập
        /// </summary>
        Task<UserSessionDto> LoginAsync(string username, string password);

        /// <summary>
        /// Lấy thông tin hồ sơ của nhân viên theo EmployeeId
        /// </summary>
        Task<UserProfileDto> GetProfileAsync(int employeeId);

        /// <summary>
        /// Cập nhật thông tin hồ sơ
        /// </summary>
        Task<bool> UpdateProfileAsync(UserProfileDto profileDto);

        /// <summary>
        /// Đổi mật khẩu tài khoản
        /// </summary>
        Task<bool> ChangePasswordAsync(int employeeId, string currentPassword, string newPassword);

        /// <summary>
        /// Khởi tạo tài khoản mặc định (cho mục đích demo/setup)
        /// </summary>
        Task<bool> CreateInitialAccountAsync(int employeeId, string username, string password);
    }
}
