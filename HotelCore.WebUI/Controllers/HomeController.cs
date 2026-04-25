using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HotelCore.WebUI.Models;
using HotelCore.Application.Interfaces;

namespace HotelCore.WebUI.Controllers;

/// <summary>
/// Controller xử lý các trang chính của ứng dụng như Dashboard, Privacy.
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDashboardService _dashboardService;

    /// <summary>
    /// Khởi tạo HomeController và tiêm (Inject) các dependencies.
    /// </summary>
    /// <param name="logger">Logger để ghi log hệ thống.</param>
    /// <param name="dashboardService">Dịch vụ xử lý logic Dashboard.</param>
    public HomeController(ILogger<HomeController> logger, IDashboardService dashboardService)
    {
        _logger = logger;
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Trang chủ (Dashboard) hiển thị số liệu thống kê và sơ đồ phòng.
    /// </summary>
    /// <returns>View Index kèm theo model DashboardOverviewDto.</returns>
    public async Task<IActionResult> Index()
    {
        // Gọi tầng Application/Infrastructure để lấy dữ liệu qua Dapper
        var dashboardData = await _dashboardService.GetDashboardOverviewAsync();
        
        // Truyền model sang View để hiển thị
        return View(dashboardData);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
