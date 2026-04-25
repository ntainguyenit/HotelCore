-- =======================================================
-- HOTEL MANAGEMENT SYSTEM DATABASE SCRIPT (SQL SERVER)
-- =======================================================

USE master;
GO

IF DB_ID('HotelCoreDB') IS NOT NULL
BEGIN
    ALTER DATABASE HotelCoreDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE HotelCoreDB;
END
GO

CREATE DATABASE HotelCoreDB;
GO

USE HotelCoreDB;
GO

-- 1. Bảng Địa Điểm (Locations)
CREATE TABLE Locations (
    LocationId INT IDENTITY(1,1) PRIMARY KEY,
    LocationName NVARCHAR(100) NOT NULL
);

-- 2. Bảng Khách Sạn (Hotels)
CREATE TABLE Hotels (
    HotelId INT IDENTITY(1,1) PRIMARY KEY,
    LocationId INT NOT NULL FOREIGN KEY REFERENCES Locations(LocationId),
    HotelName NVARCHAR(150) NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL
);

-- 3. Bảng Chức Vụ (Roles)
CREATE TABLE Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255) NULL
);

-- 4. Bảng Nhân Viên (Employees)
CREATE TABLE Employees (
    EmployeeId INT IDENTITY(1,1) PRIMARY KEY,
    HotelId INT NOT NULL FOREIGN KEY REFERENCES Hotels(HotelId),
    RoleId INT NOT NULL FOREIGN KEY REFERENCES Roles(RoleId),
    FullName NVARCHAR(100) NOT NULL,
    Gender NVARCHAR(10) NULL,
    DOB DATE NULL,
    Address NVARCHAR(255) NULL,
    Phone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100) NULL,
    BaseSalary DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Status BIT NOT NULL DEFAULT 1 -- 1: Active, 0: Inactive
);

-- 5. Bảng Khách Hàng (Customers)
CREATE TABLE Customers (
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Gender NVARCHAR(10) NULL,
    DOB DATE NULL,
    IdCardNumber NVARCHAR(20) UNIQUE NOT NULL, -- CMND/CCCD
    Address NVARCHAR(255) NULL,
    Phone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100) NULL
);

-- 6. Bảng Loại Phòng (RoomTypes)
CREATE TABLE RoomTypes (
    RoomTypeId INT IDENTITY(1,1) PRIMARY KEY,
    TypeName NVARCHAR(50) NOT NULL,
    BasePrice DECIMAL(18, 2) NOT NULL,
    Capacity INT NOT NULL, -- Số người tối đa
    Description NVARCHAR(500) NULL
);

-- 7. Bảng Phòng (Rooms)
CREATE TABLE Rooms (
    RoomId INT IDENTITY(1,1) PRIMARY KEY,
    HotelId INT NOT NULL FOREIGN KEY REFERENCES Hotels(HotelId),
    RoomTypeId INT NOT NULL FOREIGN KEY REFERENCES RoomTypes(RoomTypeId),
    RoomNumber NVARCHAR(20) NOT NULL,
    Floor INT NOT NULL,
    Area DECIMAL(8, 2) NULL, -- Diện tích m2
    Status NVARCHAR(50) NOT NULL DEFAULT 'Available' -- Available, Occupied, Cleaning, Maintenance
);

-- 8. Bảng Dịch Vụ (Services)
CREATE TABLE Services (
    ServiceId INT IDENTITY(1,1) PRIMARY KEY,
    ServiceName NVARCHAR(100) NOT NULL,
    Unit NVARCHAR(50) NOT NULL, -- Ly, Chai, Phần, Giờ, Lượt...
    Price DECIMAL(18, 2) NOT NULL,
    Status BIT NOT NULL DEFAULT 1
);

-- 9. Bảng Đặt Phòng (Bookings)
CREATE TABLE Bookings (
    BookingId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL FOREIGN KEY REFERENCES Customers(CustomerId),
    EmployeeId INT NULL FOREIGN KEY REFERENCES Employees(EmployeeId),
    BookingDate DATETIME NOT NULL DEFAULT GETDATE(),
    CheckInDate DATETIME NOT NULL,
    CheckOutDate DATETIME NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, Confirmed, CheckedIn, CheckedOut, Cancelled
    TotalAmount DECIMAL(18, 2) NULL,
    Notes NVARCHAR(500) NULL
);

-- 10. Bảng Chi Tiết Phòng Đặt (BookingRooms)
CREATE TABLE BookingRooms (
    BookingRoomId INT IDENTITY(1,1) PRIMARY KEY,
    BookingId INT NOT NULL FOREIGN KEY REFERENCES Bookings(BookingId),
    RoomId INT NOT NULL FOREIGN KEY REFERENCES Rooms(RoomId),
    Price DECIMAL(18, 2) NOT NULL -- Giá tại thời điểm đặt
);

-- 11. Bảng Dịch Vụ Khách Sử Dụng (BookingServices)
CREATE TABLE BookingServices (
    BookingServiceId INT IDENTITY(1,1) PRIMARY KEY,
    BookingId INT NOT NULL FOREIGN KEY REFERENCES Bookings(BookingId),
    ServiceId INT NOT NULL FOREIGN KEY REFERENCES Services(ServiceId),
    Quantity INT NOT NULL DEFAULT 1,
    Price DECIMAL(18, 2) NOT NULL, -- Giá tại thời điểm sử dụng
    UsageDate DATETIME NOT NULL DEFAULT GETDATE()
);

-- 12. Bảng Hóa Đơn (Invoices)
CREATE TABLE Invoices (
    InvoiceId INT IDENTITY(1,1) PRIMARY KEY,
    BookingId INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Bookings(BookingId),
    EmployeeId INT NOT NULL FOREIGN KEY REFERENCES Employees(EmployeeId),
    InvoiceDate DATETIME NOT NULL DEFAULT GETDATE(),
    RoomTotal DECIMAL(18, 2) NOT NULL DEFAULT 0,
    ServiceTotal DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Discount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    TaxAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    TotalAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    PaymentMethod NVARCHAR(50) NOT NULL -- Cash, Credit Card, Bank Transfer
);

GO

-- =======================================================
-- INSERT MOCK DATA
-- =======================================================

-- Locations
INSERT INTO Locations (LocationName) VALUES (N'Hồ Chí Minh'), (N'Hà Nội'), (N'Đà Nẵng');

-- Hotels
INSERT INTO Hotels (LocationId, HotelName, Address, Phone, Email) 
VALUES (1, N'HotelCore Sài Gòn', N'123 Lê Lợi, Q.1, TP.HCM', '0123456789', 'sg@hotelcore.com');

-- Roles
INSERT INTO Roles (RoleName, Description) 
VALUES (N'Quản lý', N'Quản lý chi nhánh khách sạn'),
       (N'Lễ tân', N'Nhân viên lễ tân (Front Desk)'),
       (N'Kế toán', N'Nhân viên kế toán');

-- Employees
INSERT INTO Employees (HotelId, RoleId, FullName, Gender, DOB, Address, Phone, Email, BaseSalary)
VALUES (1, 1, N'Nguyễn Văn A', N'Nam', '1985-05-15', N'TP.HCM', '0901111111', 'nva@hotelcore.com', 20000000),
       (1, 2, N'Trần Thị B', N'Nữ', '1998-10-20', N'TP.HCM', '0902222222', 'ttb@hotelcore.com', 10000000);

-- Customers
INSERT INTO Customers (FullName, Gender, DOB, IdCardNumber, Address, Phone, Email)
VALUES (N'Lê Hoàng C', N'Nam', '1990-01-01', '079090012345', N'Đà Nẵng', '0988888888', 'lhc@gmail.com'),
       (N'Phạm Thị D', N'Nữ', '1995-12-12', '079095054321', N'Hà Nội', '0977777777', 'ptd@gmail.com');

-- RoomTypes
INSERT INTO RoomTypes (TypeName, BasePrice, Capacity, Description)
VALUES (N'Standard (STD)', 500000, 2, N'Phòng tiêu chuẩn 1 giường đôi'),
       (N'Superior (SUP)', 800000, 2, N'Phòng rộng có view thành phố'),
       (N'Deluxe (DLX)', 1200000, 3, N'Phòng cao cấp 1 giường đôi 1 giường đơn'),
       (N'Suite (SUT)', 2500000, 4, N'Phòng VIP diện tích lớn, nội thất sang trọng');

-- Rooms
INSERT INTO Rooms (HotelId, RoomTypeId, RoomNumber, Floor, Area, Status)
VALUES (1, 1, '101', 1, 20.0, 'Available'),
       (1, 1, '102', 1, 20.0, 'Occupied'),
       (1, 2, '201', 2, 28.0, 'Cleaning'),
       (1, 3, '301', 3, 35.0, 'Maintenance'),
       (1, 4, '401', 4, 50.0, 'Available');

-- Services
INSERT INTO Services (ServiceName, Unit, Price)
VALUES (N'Giặt ủi', N'Bộ', 50000),
       (N'Mì xào hải sản', N'Phần', 80000),
       (N'Nước suối', N'Chai', 15000),
       (N'Đưa đón sân bay', N'Lượt', 300000);

-- Bookings
INSERT INTO Bookings (CustomerId, EmployeeId, BookingDate, CheckInDate, CheckOutDate, Status)
VALUES (1, 2, GETDATE()-5, GETDATE()-2, GETDATE()+1, 'CheckedIn'),
       (2, 2, GETDATE()-1, GETDATE()+5, GETDATE()+7, 'Confirmed');

-- BookingRooms
INSERT INTO BookingRooms (BookingId, RoomId, Price)
VALUES (1, 2, 500000), -- Khách C ở phòng 102 (Standard)
       (2, 5, 2500000); -- Khách D đặt phòng 401 (Suite)

-- BookingServices
INSERT INTO BookingServices (BookingId, ServiceId, Quantity, Price, UsageDate)
VALUES (1, 2, 2, 80000, GETDATE()-1), -- 2 Mì xào
       (1, 3, 4, 15000, GETDATE()-1); -- 4 Nước suối

PRINT 'Database HotelCoreDB created successfully with sample data!';
GO
