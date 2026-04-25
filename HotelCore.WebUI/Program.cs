var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Khai báo Dependency Injection (DI) theo chuẩn Clean Architecture
builder.Services.AddScoped<HotelCore.Application.Interfaces.IDashboardService, HotelCore.Infrastructure.Services.DashboardService>();
builder.Services.AddScoped<HotelCore.Application.Interfaces.IBookingService, HotelCore.Infrastructure.Services.BookingService>();
builder.Services.AddScoped<HotelCore.Application.Interfaces.ICustomerService, HotelCore.Infrastructure.Services.CustomerService>();
builder.Services.AddScoped<HotelCore.Application.Interfaces.IEmployeeService, HotelCore.Infrastructure.Services.EmployeeService>();
builder.Services.AddScoped<HotelCore.Application.Interfaces.IInvoiceService, HotelCore.Infrastructure.Services.InvoiceService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
