using System;
using System.Threading.Tasks;
using HotelCore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelCore.WebUI.Controllers
{
    [Authorize(Roles = "Quản lý")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> Index(string type = "7days", int? year = null, int? month = null)
        {
            DateTime startDate;
            DateTime endDate = DateTime.Now;

            if (type == "month")
            {
                int y = year ?? DateTime.Now.Year;
                int m = month ?? DateTime.Now.Month;
                startDate = new DateTime(y, m, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
                ViewBag.Title = $"Báo cáo tháng {m}/{y}";
            }
            else if (type == "year")
            {
                int y = year ?? DateTime.Now.Year;
                startDate = new DateTime(y, 1, 1);
                endDate = new DateTime(y, 12, 31);
                ViewBag.Title = $"Báo cáo năm {y}";
            }
            else // mặc định 7 ngày
            {
                startDate = DateTime.Now.AddDays(-7);
                ViewBag.Title = "Báo cáo 7 ngày gần nhất";
            }

            var analytics = await _reportService.GetAnalyticsOverviewAsync(startDate, endDate);
            
            ViewBag.ActiveType = type;
            ViewBag.SelectedYear = year ?? DateTime.Now.Year;
            ViewBag.SelectedMonth = month ?? DateTime.Now.Month;

            return View(analytics);
        }

        // Action phụ để lấy dữ liệu JSON cho AJAX nếu cần (tương lai)
        public async Task<JsonResult> GetChartData(int year)
        {
            var data = await _reportService.GetYearlyAnalyticsAsync(year);
            return Json(data);
        }
    }
}
