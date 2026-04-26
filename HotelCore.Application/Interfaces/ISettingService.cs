using System.Threading.Tasks;
using HotelCore.Application.DTOs;

namespace HotelCore.Application.Interfaces
{
    public interface ISettingService
    {
        /// <summary>
        /// Lấy toàn bộ cài đặt hệ thống
        /// </summary>
        Task<SystemSettingsDto> GetSystemSettingsAsync();

        /// <summary>
        /// Cập nhật cài đặt hệ thống
        /// </summary>
        Task<bool> UpdateSystemSettingsAsync(SystemSettingsDto settingsDto);

        /// <summary>
        /// Lấy một giá trị cài đặt cụ thể theo Key
        /// </summary>
        Task<string> GetSettingValueAsync(string key);
    }
}
