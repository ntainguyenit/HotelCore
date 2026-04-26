using System;
using System.Threading.Tasks;
using HotelCore.Application.DTOs;

namespace HotelCore.Application.Interfaces
{
    public interface IReportService
    {
        /// <summary>
        /// Lấy dữ liệu tổng quan cho Dashboard báo cáo
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu lọc</param>
        /// <param name="endDate">Ngày kết thúc lọc</param>
        Task<AnalyticsOverviewDto> GetAnalyticsOverviewAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Thống kê doanh thu theo năm (theo từng tháng)
        /// </summary>
        Task<AnalyticsOverviewDto> GetYearlyAnalyticsAsync(int year);
    }
}
