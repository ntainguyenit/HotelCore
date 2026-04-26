var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Khai báo Dependency Injection (DI) theo chuẩn Clean Architecture
builder.Services.AddScoped<HotelCore.Application.Interfaces.IDashboardService, HotelCore.Infrastructure.Services.DashboardService>();
builder.Services.AddScoped<HotelCore.Application.Interfaces.IBookingService, HotelCore.Infrastructure.Services.BookingService>();
builder.Services.AddScoped<HotelCore.Application.Interfaces.ICustomerService, HotelCore.Infrastructure.Services.CustomerService>();
builder.Services.AddScoped<HotelCore.Application.Interfaces.IEmployeeService, HotelCore.Infrastructure.Services.EmployeeService>();
builder.Services.AddScoped<HotelCore.Application.Interfaces.IInvoiceService, HotelCore.Infrastructure.Services.InvoiceService>();
builder.Services.AddScoped<HotelCore.Application.Interfaces.IServiceService, HotelCore.Infrastructure.Services.ServiceService>();
builder.Services.AddScoped<HotelCore.Application.Interfaces.IAuthService, HotelCore.Infrastructure.Services.AuthService>();
builder.Services.AddScoped<HotelCore.Application.Interfaces.ISettingService, HotelCore.Infrastructure.Services.SettingService>();

// Cấu hình Authentication
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.Name = "HotelCore.Auth";
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
