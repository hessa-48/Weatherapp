-- ================================================
-- WeatherApp Database Script
-- ================================================

-- 1️⃣ Drop the database if it exists (optional)
IF DB_ID('WeatherAppDB') IS NOT NULL
BEGIN
    DROP DATABASE WeatherAppDB;
END
GO

-- 2️⃣ Create the database
CREATE DATABASE WeatherAppDB;
GO

-- 3️⃣ Use the database
USE WeatherAppDB;
GO

-- 4️⃣ Create Users table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(50) NOT NULL
);
GO

-- 5️⃣ Create WeatherResults table
CREATE TABLE WeatherResults (
    ResultId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    City NVARCHAR(50) NOT NULL,
    Humidity INT NULL,
    TempMin FLOAT NULL,
    TempMax FLOAT NULL,
    DateAdded DATETIME NOT NULL,
    DateRecorded DATETIME NOT NULL
);
GO

-- 6️⃣ Stored Procedure: Login User
CREATE PROCEDURE sp_LoginUser
    @Username NVARCHAR(50),
    @Password NVARCHAR(50)
AS
BEGIN
    SELECT UserId, Username, Password
    FROM Users
    WHERE Username = @Username AND Password = @Password;
END
GO

-- 7️⃣ Stored Procedure: Add Weather Result
CREATE PROCEDURE sp_AddWeatherResult
    @UserId INT,
    @City NVARCHAR(50),
    @Humidity INT,
    @TempMin FLOAT,
    @TempMax FLOAT,
    @Date DATETIME
AS
BEGIN
    INSERT INTO WeatherResults (UserId, City, Humidity, TempMin, TempMax, DateRecorded, DateAdded)
    VALUES (@UserId, @City, @Humidity, @TempMin, @TempMax, @Date, GETDATE());
END
GO

-- 8️⃣ Stored Procedure: Update Weather Result
CREATE PROCEDURE sp_UpdateWeatherResult
    @ResultId INT,
    @City NVARCHAR(50),
    @Humidity INT,
    @TempMin FLOAT,
    @TempMax FLOAT,
    @Date DATETIME
AS
BEGIN
    UPDATE WeatherResults
    SET City = @City,
        Humidity = @Humidity,
        TempMin = @TempMin,
        TempMax = @TempMax,
        DateRecorded = @Date
    WHERE ResultId = @ResultId;
END
GO

-- 9️⃣ Stored Procedure: Get User Weather Results
CREATE PROCEDURE sp_GetUserWeatherResults
    @UserId INT
AS
BEGIN
    SELECT *
    FROM WeatherResults
    WHERE UserId = @UserId
    ORDER BY DateRecorded DESC;
END
GO

--  🔹 Optional: Insert a test user
INSERT INTO Users (Username, Password)
VALUES ('testuser', 'password123');
GO
