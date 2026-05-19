[English Below]

# HotelCore - Hệ Thống Quản Lý Khách Sạn Doanh Nghiệp

**HotelCore** là một Hệ thống Quản lý Khách sạn toàn diện (full-stack) được xây dựng bằng các công nghệ web hiện đại và các phương pháp kỹ thuật phần mềm tốt nhất. Dự án được thiết kế theo mô hình **Kiến trúc Sạch (Clean Architecture / Onion Architecture)** nhằm đảm bảo khả năng bảo trì cao, dễ dàng kiểm thử và phân tách rõ ràng các thành phần hệ thống.

## Các Tính Năng Chính
- **Tổng Quan Dashboard:** Thống kê thời gian thực về tình trạng phòng trống, tỷ lệ lấp đầy phòng và doanh thu hàng ngày.
- **Sơ Đồ Phòng Trực Quan:** Lưới tương tác biểu diễn các trạng thái phòng (Trống, Đang có khách, Đang dọn dẹp, Bảo trì) theo tầng và loại phòng.
- **Quản Lý Đặt Phòng:** Quản lý quy trình nhận phòng (Check-in), trả phòng (Check-out) và vòng đời đặt phòng.
- **Quản Lý Dịch Vụ:** Theo dõi các dịch vụ khách sạn mà khách hàng sử dụng (ví dụ: Ăn uống, Giặt ủi).
- **Hóa Đơn & Thanh Toán:** Tính toán hóa đơn tự động chi tiết kết hợp tiền phòng và các dịch vụ đi kèm.

## Thiết Kế Kiến Trúc (Clean Architecture)
Để đảm bảo ứng dụng có khả năng mở rộng tốt và giảm thiểu sự phụ thuộc lẫn nhau, dự án được chia thành 4 lớp chính:
1. **`HotelCore.Domain`:** Cốt lõi của ứng dụng chứa các Thực thể Doanh nghiệp (Models) và các Giao diện Nghiệp vụ (Domain Interfaces). *Không phụ thuộc vào bất kỳ thư viện bên ngoài nào.*
2. **`HotelCore.Application`:** Chứa logic nghiệp vụ, các DTO và Giao diện cho Kho lưu trữ (Repositories) cũng như Dịch vụ (Services).
3. **`HotelCore.Infrastructure`:** Triển khai logic truy cập dữ liệu sử dụng **Dapper** (Micro-ORM) để tối ưu hóa việc thực thi các câu lệnh SQL thuần túy truy vấn vào **SQL Server**.
4. **`HotelCore.WebUI`:** Lớp Trực quan hóa (Presentation Layer) sử dụng **ASP.NET Core MVC**. Phần giao diện người dùng được hỗ trợ bởi **Bootstrap 5** và **jQuery** với giao diện màu Cam Đậm (Dark Orange) hiện đại được tùy chỉnh.

## Công Nghệ Sử Dụng
- **Backend:** C#, .NET 8, ASP.NET Core MVC
- **Database:** Microsoft SQL Server
- **ORM:** Dapper (Tối ưu hiệu suất hơn EF Core đối với các truy vấn phức tạp)
- **Frontend:** HTML5, CSS3, Bootstrap 5, jQuery, Bootstrap Icons
- **Design Pattern:** Repository Pattern, Dependency Injection (DI)

## Sơ Đồ Cơ Sở Dữ Liệu
Cơ sở dữ liệu quan hệ được chuẩn hóa hoàn toàn và bao gồm các bảng chính sau:
- `Hotels` (Khách sạn), `Locations` (Vị trí)
- `Rooms` (Phòng), `RoomTypes` (Loại phòng)
- `Customers` (Khách hàng), `Employees` (Nhân viên), `Roles` (Vai trò)
- `Bookings` (Đặt phòng), `BookingRooms` (Phòng đặt), `BookingServices` (Dịch vụ đặt), `Invoices` (Hóa đơn)

*(Mã nguồn đã đi kèm sẵn tệp kịch bản `Database.sql` với dữ liệu giả lập để có thể thử nghiệm ngay lập tức).*

## Hướng Dẫn Chạy Dự Án Dưới Local
1. **Clone repository:**
   ```bash
   git clone https://github.com/your-username/HotelCore.git
   ```
2. **Khởi Tạo Cơ Sở Dữ Liệu:**
   - Mở SQL Server Management Studio (SSMS).
   - Thực thi tệp `Database.sql` nằm ở thư mục gốc. Thao tác này sẽ tự động tạo cơ sở dữ liệu `HotelCoreDB` và nạp dữ liệu mẫu vào.
3. **Cấu Hình Chuỗi Kết Nối:**
   - Mở `HotelCore.WebUI/appsettings.json`.
   - Cập nhật chuỗi kết nối `DefaultConnection` với thông tin đăng nhập thực tế của phiên bản SQL Server của bạn.
4. **Chạy Ứng Dụng:**
   - Mở giải pháp `HotelCore.sln` bằng **Visual Studio 2022**.
   - Đảm bảo `HotelCore.WebUI` được thiết lập làm **Startup Project** (Dự án khởi động).
   - Nhấn `F5` để chạy ứng dụng thông qua HTTPS hoặc IIS Express.

## Tác Giả
**Tai Nguyen - Nguyễn Ngọc Thanh Tài**

---

# HotelCore - Enterprise Hotel Management System

**HotelCore** is a comprehensive, full-stack Hotel Management System built with modern web technologies and software engineering best practices. The project is designed with **Clean Architecture (Onion Architecture)** to ensure high maintainability, testability, and separation of concerns.

## Key Features
- **Dashboard Overview:** Real-time statistics of room availability, occupancy rate, and daily revenue.
- **Visual Room Layout:** Interactive grid representing room states (Available, Occupied, Cleaning, Maintenance) based on floors and room types.
- **Booking Management:** Check-in, check-out, and booking lifecycle management.
- **Service Management:** Tracking hotel services consumed by guests (e.g., F&B, Laundry).
- **Billing & Invoicing:** Detailed automated billing calculation combining room charges and services.

## Architecture Design (Clean Architecture)
To ensure the application scales well and remains decoupled, it is divided into 4 main layers:
1. **`HotelCore.Domain`:** The core of the application containing Business Entities (Models) and Domain Interfaces. *Has no external dependencies.*
2. **`HotelCore.Application`:** Contains business logic, DTOs, and Interfaces for Repositories and Services.
3. **`HotelCore.Infrastructure`:** Implementation of data access logic using **Dapper** (Micro-ORM) for highly optimized raw SQL execution against **SQL Server**.
4. **`HotelCore.WebUI`:** The Presentation Layer using **ASP.NET Core MVC**. The frontend is powered by **Bootstrap 5** and **jQuery** with a customized modern Dark Orange theme.

## Technology Stack
- **Backend:** C#, .NET 8, ASP.NET Core MVC
- **Database:** Microsoft SQL Server
- **ORM:** Dapper (Optimized performance over EF Core for complex queries)
- **Frontend:** HTML5, CSS3, Bootstrap 5, jQuery, Bootstrap Icons
- **Design Pattern:** Repository Pattern, Dependency Injection (DI)

## Database Schema
The relational database is fully normalized and includes the following key tables:
- `Hotels`, `Locations`
- `Rooms`, `RoomTypes`
- `Customers`, `Employees`, `Roles`
- `Bookings`, `BookingRooms`, `BookingServices`, `Invoices`

*(The repository includes a ready-to-run `Database.sql` script with mock data for instant testing).*

## How to Run the Project Locally
1. **Clone the repository:**
   ```bash
   git clone https://github.com/your-username/HotelCore.git
   ```
2. **Initialize Database:**
   - Open SQL Server Management Studio (SSMS).
   - Execute the `Database.sql` file located in the root directory. This will automatically create the database `HotelCoreDB` and populate it with sample data.
3. **Configure Connection String:**
   - Open `HotelCore.WebUI/appsettings.json`.
   - Update the `DefaultConnection` string with your SQL Server instance credentials.
4. **Run the Application:**
   - Open the solution `HotelCore.sln` in **Visual Studio 2022**.
   - Ensure `HotelCore.WebUI` is set as the **Startup Project**.
   - Press `F5` to run via HTTPS or IIS Express.

## Author
**Tai Nguyen - Nguyen Ngoc Thanh Tai**
