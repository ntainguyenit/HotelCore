USE HotelCoreDB;
GO

IF OBJECT_ID('Settings', 'U') IS NULL 
BEGIN
    CREATE TABLE Settings (
        SettingId INT IDENTITY(1,1) PRIMARY KEY,
        SettingKey NVARCHAR(100) NOT NULL UNIQUE,
        SettingValue NVARCHAR(MAX) NULL,
        Description NVARCHAR(255) NULL,
        GroupName NVARCHAR(50) NULL
    );

    INSERT INTO Settings (SettingKey, SettingValue, Description, GroupName) VALUES
    ('HotelName', N'HotelCore Management', N'Tên khách sạn hiển thị trên hệ thống', 'General'),
    ('HotelAddress', N'123 Lê Lợi, Q.1, TP.HCM', N'Địa chỉ khách sạn', 'General'),
    ('HotelPhone', '0123456789', N'Số điện thoại liên hệ', 'General'),
    ('HotelEmail', 'contact@hotelcore.com', N'Email liên hệ', 'General'),
    ('VatRate', '10', N'Thuế suất giá trị gia tăng (%)', 'Financial'),
    ('Currency', 'VND', N'Đơn vị tiền tệ chính', 'Financial'),
    ('ServiceFee', '0', N'Phí phục vụ mặc định (%)', 'Financial'),
    ('LogoPath', '/images/logo.png', N'Đường dẫn file logo', 'Branding');
END
GO
