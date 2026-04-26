USE HotelCoreDB;
GO

IF OBJECT_ID('Accounts', 'U') IS NULL 
BEGIN
    CREATE TABLE Accounts (
        AccountId INT IDENTITY(1,1) PRIMARY KEY,
        EmployeeId INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Employees(EmployeeId),
        Username NVARCHAR(50) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        CreatedAt DATETIME DEFAULT GETDATE(),
        LastLogin DATETIME NULL
    );
END
GO
