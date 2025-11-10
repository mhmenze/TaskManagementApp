-- =============================================
-- Database Creation
-- =============================================
CREATE DATABASE TaskManagementDB;
GO

USE TaskManagementDB;
GO

-- =============================================
-- Table: Users
-- =============================================
CREATE TABLE Users (
    UserID BIGINT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    MiddleName NVARCHAR(100) NULL,
    LastName NVARCHAR(100) NOT NULL,
    DisplayName NVARCHAR(200) NULL,
    UserRole VARCHAR(20) NOT NULL DEFAULT 'user',
    Username VARCHAR(50) NOT NULL UNIQUE,
    [Password] VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL UNIQUE,
    CreatedOn DATETIME NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100) NULL,
    UpdatedOn DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy NVARCHAR(100) NULL,
    [Status] BIT NOT NULL DEFAULT 1
);
GO

-- =============================================
-- Table: Tasks
-- =============================================
CREATE TABLE Tasks (
    TaskID BIGINT IDENTITY(1,1) PRIMARY KEY,
    TaskName NVARCHAR(200) NOT NULL,
    TaskDescription NVARCHAR(1000) NULL,
    AssignedUserIDs NVARCHAR(MAX) NULL, -- JSON array of user IDs
    Status INT NOT NULL DEFAULT 0, -- 0=ToDo, 1=InProgress, 2=Completed, 3=UnAssigned, 4=Deleted
    IsDelayed BIT NOT NULL DEFAULT 0,
    Deadline DATETIME NULL,
    CreatedOn DATETIME NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100) NULL,
    UpdatedOn DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy NVARCHAR(100) NULL
);
GO

-- =============================================
-- Table: UserNotifications
-- =============================================
CREATE TABLE UserNotifications (
    NotificationID BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserID BIGINT NOT NULL,
    [Type] INT NOT NULL, -- 0=Email, 1=SMS, 2=BrowserAlert, 3=PushNotification, 4=InAppMessage
    [Target] NVARCHAR(500) NOT NULL,
    [Message] NVARCHAR(1000) NOT NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100) NULL,
    UpdatedOn DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy NVARCHAR(100) NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO