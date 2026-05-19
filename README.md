[English Below]

# HotelCore - Hệ Thống Quản Lý Khách Sạn Doanh Nghiệp

**HotelCore** là một Hệ thống Quản lý Khách sạn toàn diện (full-stack) được xây dựng bằng các công nghệ web hiện đại và các phương pháp kỹ thuật phần mềm tốt nhất. Dự án được thiết kế theo mô hình **Kiến trúc Sạch (Clean Architecture / Onion Architecture)** nhằm đảm bảo khả năng bảo trì cao, dễ dàng kiểm thử và phân tách rõ ràng các thành phần hệ thống.

## Các Tính Năng Chính
- **Tổng Quan Dashboard:** Thống kê thời gian thực về tình trạng phòng trống, tỷ lệ lấp đầy phòng và doanh thu hàng ngày.
- **Sơ Đồ Phòng Trực Quan:** Lưới tương tác biểu diễn các trạng thái phòng (Trống, Đang có khách, Đang dọn dẹp, Bảo trì) theo tầng và loại phòng.
- **Quản Lý Đặt Phòng:** Quản lý quy trình nhận phòng (Check-in), trả phòng (Check-out) và vòng đời đặt phòng.
- **Quản Lý Dịch Vụ:** Theo dõi các dịch vụ khách sạn mà khách hàng sử dụng (ví dụ: Ăn uống, Giặt ủi).
- **Hóa Đơn & Thanh Toán:** Tính toán hóa đơn tự động chi tiết kết hợp tiền phòng và các dịch vụ đi kèm.
- **Hệ Thống Khuyến Mãi Động (Promotion Engine):** Tự động tính toán chiết khấu tối ưu kết hợp từ thời gian lưu trú (Long stay discount), phân hạng thành viên của khách hàng, và các mã ưu đãi đặc biệt (Promo Code) với giới hạn biên an toàn.
- **Chương Trình Khách Hàng Thân Thiết (Loyalty Program):** Quản lý điểm thưởng tích lũy động từ doanh thu tiền phòng & dịch vụ phụ trợ, tự động nâng cấp phân hạng thành viên (Silver -> Gold -> Platinum) kèm hệ số nhân điểm thưởng.
- **Phân Tích Doanh Thu Chuyên Sâu (Revenue Management System):** Đo lường các chỉ số tài chính khách sạn cốt lõi như công suất lấp đầy phòng, ADR (Average Daily Rate), RevPAR (Revenue Per Available Room) và dự báo xu hướng công suất phòng tương lai.
- **Xác Thực Đặt Phòng Chống Trùng Lịch (Booking Validation):** Tự động phát hiện xung đột lịch đặt phòng sử dụng đối tượng giá trị `DateRange`, khống chế số lượng ngày đặt tối đa và ngăn chặn đặt các phòng đang bảo trì.
- **Bộ Kiểm Thử Toàn Diện (Comprehensive xUnit Tests):** Bộ hơn 44 Unit Tests và Integration Tests bao phủ toàn bộ các kịch bản nghiệp vụ cốt lõi, bảo vệ tối đa tính toàn vẹn hệ thống.

## Thiết Kế Kiến Trúc (Clean Architecture)
Để đảm bảo ứng dụng có khả năng mở rộng tốt và giảm thiểu sự phụ thuộc lẫn nhau, dự án được chia thành các lớp chính:
1. **`HotelCore.Domain`:** Cốt lõi của ứng dụng chứa các Thực thể Doanh nghiệp (Customer, Room, Booking, Invoice, Service, Promotion), Đối tượng giá trị (`DateRange` Value Object), các Enums và Custom Exceptions. *Không phụ thuộc vào bất kỳ thư viện bên ngoài nào.*
2. **`HotelCore.Application`:** Chứa logic nghiệp vụ, các DTO, các Giao diện (Interfaces), và triển khai các dịch vụ tính toán cốt lõi (`PromotionEngine`, `LoyaltyService`, `RevenueManager`, `BookingValidationService`).
3. **`HotelCore.Infrastructure`:** Triển khai logic truy cập dữ liệu sử dụng **Dapper** (Micro-ORM) để tối ưu hóa việc thực thi các câu lệnh SQL thuần túy truy vấn vào **SQL Server**.
4. **`HotelCore.WebUI`:** Lớp Trực quan hóa (Presentation Layer) sử dụng **ASP.NET Core MVC**. Phần giao diện người dùng được hỗ trợ bởi **Bootstrap 5** và **jQuery** với giao diện màu Cam Đậm (Dark Orange) hiện đại được tùy chỉnh.
5. **`HotelCore.Tests` (Dự Án Mới):** Dự án kiểm thử sử dụng thư viện **xUnit**, kiểm thử độc lập toàn bộ các quy tắc nghiệp vụ tầng Domain & Application và giả lập giao dịch tích hợp đầu cuối.

## Công Nghệ Sử Dụng
- **Backend:** C#, .NET 8, ASP.NET Core MVC
- **Database:** Microsoft SQL Server
- **ORM:** Dapper (Tối ưu hiệu suất hơn EF Core đối với các truy vấn phức tạp)
- **Testing:** xUnit, .NET Test SDK (44 bộ tests, 100% tỷ lệ đỗ)
- **Frontend:** HTML5, CSS3, Bootstrap 5, jQuery, Bootstrap Icons
- **Design Pattern:** Domain-Driven Design (DDD) elements, Repository Pattern, Dependency Injection (DI)

## Sơ Đồ Cơ Sở Dữ Liệu
Cơ sở dữ liệu quan hệ được chuẩn hóa hoàn toàn và bao gồm các bảng chính sau:
- `Hotels` (Khách sạn), `Locations` (Vị trí)
- `Rooms` (Phòng), `RoomTypes` (Loại phòng)
- `Customers` (Khách hàng), `Employees` (Nhân viên), `Roles` (Vai trò)
- `Bookings` (Đặt phòng), `BookingRooms` (Phòng đặt), `BookingServices` (Dịch vụ đặt), `Invoices` (Hóa đơn)

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
4. **Chạy Các Bộ Kiểm Thử (Tests):**
   - Di chuyển vào thư mục dự án và chạy lệnh test để xác nhận logic hoạt động bình thường:
     ```bash
     dotnet test
     ```
5. **Chạy Ứng Dụng:**
   - Mở giải pháp `HotelCore.sln` bằng **Visual Studio 2022**.
   - Đảm bảo `HotelCore.WebUI` được thiết lập làm **Startup Project** (Dự án khởi động).
   - Nhấn `F5` để chạy ứng dụng thông qua HTTPS hoặc IIS Express.

---

# HotelCore - Enterprise Hotel Management System

**HotelCore** is a comprehensive, full-stack Hotel Management System built with modern web technologies and software engineering best practices. The project is designed with **Clean Architecture (Onion Architecture)** to ensure high maintainability, testability, and separation of concerns.

## Key Features
- **Dashboard Overview:** Real-time statistics of room availability, occupancy rate, and daily revenue.
- **Visual Room Layout:** Interactive grid representing room states (Available, Occupied, Cleaning, Maintenance) based on floors and room types.
- **Booking Management:** Check-in, check-out, and booking lifecycle management.
- **Service Management:** Tracking hotel services consumed by guests (e.g., F&B, Laundry).
- **Billing & Invoicing:** Detailed automated billing calculation combining room charges and services.
- **Dynamic Promotion Engine:** Automatically applies optimized discount rates combining length of stay (Long stay discount), customer loyalty tiers, and special promo codes with a safe margin cap.
- **Loyalty & Rewards Program:** Dynamically calculates reward points based on room booking costs and auxiliary service usages with tier multipliers, automatically upgrading member status (Silver -> Gold -> Platinum).
- **Advanced Revenue Management System:** Measures core hospitality financial metrics including occupancy rate, ADR (Average Daily Rate), RevPAR (Revenue Per Available Room), and future occupancy trend forecasting.
- **Booking Conflict Validation:** Ensures rooms are not double-booked using the `DateRange` Value Object, blocks bookings on maintenance rooms, and validates stay duration limits.
- **Comprehensive xUnit Test Suite:** Over 44 unit and integration tests covering the core domain rules and application engines, protecting the system against regressions.

## Architecture Design (Clean Architecture)
To ensure the application scales well and remains decoupled, it is divided into the following layers:
1. **`HotelCore.Domain`:** The core of the application containing Business Entities (Customer, Room, Booking, Invoice, Service, Promotion), Value Objects (`DateRange` Value Object), Enums, and Domain Custom Exceptions. *Has no external dependencies.*
2. **`HotelCore.Application`:** Contains business logic, DTOs, Interfaces, and concrete implementations of core engines (`PromotionEngine`, `LoyaltyService`, `RevenueManager`, `BookingValidationService`).
3. **`HotelCore.Infrastructure`:** Implementation of data access logic using **Dapper** (Micro-ORM) for highly optimized raw SQL execution against **SQL Server**.
4. **`HotelCore.WebUI`:** The Presentation Layer using **ASP.NET Core MVC**. The frontend is powered by **Bootstrap 5** and **jQuery** with a customized modern Dark Orange theme.
5. **`HotelCore.Tests` (New Project):** The xUnit testing project containing comprehensive unit and integration tests to verify all core business engines and domain validation invariants.

## Technology Stack
- **Backend:** C#, .NET 8, ASP.NET Core MVC
- **Database:** Microsoft SQL Server
- **ORM:** Dapper (Optimized performance over EF Core for complex queries)
- **Testing:** xUnit, .NET Test SDK (44 test cases, 100% pass rate)
- **Frontend:** HTML5, CSS3, Bootstrap 5, jQuery, Bootstrap Icons
- **Design Pattern:** Domain-Driven Design (DDD) elements, Repository Pattern, Dependency Injection (DI)

## Database Schema
The relational database is fully normalized and includes the following key tables:
- `Hotels`, `Locations`
- `Rooms`, `RoomTypes`
- `Customers`, `Employees`, `Roles`
- `Bookings`, `BookingRooms`, `BookingServices`, `Invoices`

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
4. **Run the Test Suite:**
   - Run the following command in the solution directory to execute all unit and integration tests:
     ```bash
     dotnet test
     ```
5. **Run the Application:**
   - Open the solution `HotelCore.sln` in **Visual Studio 2022**.
   - Ensure `HotelCore.WebUI` is set as the **Startup Project**.
   - Press `F5` to run via HTTPS or IIS Express.