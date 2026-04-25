# HotelCore - Enterprise Hotel Management System

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![.NET Version](https://img.shields.io/badge/.NET-8.0-blueviolet)
![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-orange)
![ORM](https://img.shields.io/badge/ORM-Dapper-lightgrey)

**HotelCore** is a comprehensive, full-stack Hotel Management System built with modern web technologies and software engineering best practices. The project is designed with **Clean Architecture (Onion Architecture)** to ensure high maintainability, testability, and separation of concerns.

## 🚀 Key Features
- **Dashboard Overview:** Real-time statistics of room availability, occupancy rate, and daily revenue.
- **Visual Room Layout:** Interactive grid representing room states (Available, Occupied, Cleaning, Maintenance) based on floors and room types.
- **Booking Management:** Check-in, check-out, and booking lifecycle management.
- **Service Management:** Tracking hotel services consumed by guests (e.g., F&B, Laundry).
- **Billing & Invoicing:** Detailed automated billing calculation combining room charges and services.

## 🏗️ Architecture Design (Clean Architecture)
To ensure the application scales well and remains decoupled, it is divided into 4 main layers:
1. **`HotelCore.Domain`:** The core of the application containing Business Entities (Models) and Domain Interfaces. *Has no external dependencies.*
2. **`HotelCore.Application`:** Contains business logic, DTOs, and Interfaces for Repositories and Services.
3. **`HotelCore.Infrastructure`:** Implementation of data access logic using **Dapper** (Micro-ORM) for highly optimized raw SQL execution against **SQL Server**.
4. **`HotelCore.WebUI`:** The Presentation Layer using **ASP.NET Core MVC**. The frontend is powered by **Bootstrap 5** and **jQuery** with a customized modern Dark Orange theme.

## 🛠️ Technology Stack
- **Backend:** C#, .NET 8, ASP.NET Core MVC
- **Database:** Microsoft SQL Server
- **ORM:** Dapper (Optimized performance over EF Core for complex queries)
- **Frontend:** HTML5, CSS3, Bootstrap 5, jQuery, Bootstrap Icons
- **Design Pattern:** Repository Pattern, Dependency Injection (DI)

## 🗄️ Database Schema
The relational database is fully normalized and includes the following key tables:
- `Hotels`, `Locations`
- `Rooms`, `RoomTypes`
- `Customers`, `Employees`, `Roles`
- `Bookings`, `BookingRooms`, `BookingServices`, `Invoices`

*(The repository includes a ready-to-run `Database.sql` script with mock data for instant testing).*

## ⚙️ How to Run the Project Locally
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

## 👨‍💻 Author
**Software Engineer Intern Candidate**  
Passionate about building scalable backend systems and responsive web applications.

---
*This project demonstrates proficiency in standard enterprise design patterns, database design, and end-to-end full-stack web development.*
