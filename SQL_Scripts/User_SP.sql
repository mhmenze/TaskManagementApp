-- =============================================
-- Author:        Nazeer
-- Create date:   2025-11-09
-- Description:   Stored Procedures for User Management
-- =============================================

-- =============================================
-- sp_GetAllUsers
-- =============================================
IF OBJECT_ID('dbo.sp_GetAllUsers', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetAllUsers;
GO
CREATE PROCEDURE dbo.sp_GetAllUsers
AS
BEGIN
    SET NOCOUNT ON;

    SELECT UserID, FirstName, MiddleName, LastName, DisplayName, Username, Password, 
            UserRole, Email, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy
    FROM Users
    WHERE [Status] = 1
    ORDER BY FirstName;
END
GO

-- =============================================
-- sp_GetUserById
-- =============================================
IF OBJECT_ID('dbo.sp_GetUserById', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetUserById;
GO
CREATE PROCEDURE dbo.sp_GetUserById
    @UserID BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT UserID, FirstName, MiddleName, LastName, DisplayName, Username, Password, 
            UserRole, Email, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy
    FROM Users
    WHERE UserID = @UserID;
END
GO

-- =============================================
-- sp_GetUserByUsername
-- =============================================
IF OBJECT_ID('dbo.sp_GetUserByUsername', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetUserByUsername;
GO
CREATE PROCEDURE dbo.sp_GetUserByUsername
    @Username NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT UserID, FirstName, MiddleName, LastName, DisplayName, Username, Password, 
            UserRole, Email, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy
    FROM Users
    WHERE Username = @Username AND [Status] = 1;
END
GO

-- =============================================
-- sp_CreateUser
-- =============================================
IF OBJECT_ID('dbo.sp_CreateUser', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CreateUser;
GO
CREATE PROCEDURE dbo.sp_CreateUser
    @FirstName NVARCHAR(100),
    @MiddleName NVARCHAR(100) = NULL,
    @LastName NVARCHAR(100),
    @DisplayName NVARCHAR(200) = NULL,
    @Username NVARCHAR(50),
    @Password NVARCHAR(255),
    @UserRole VARCHAR(20),
    @Email NVARCHAR(255),
    @CreatedBy NVARCHAR(100) = NULL,
    @UpdatedBy NVARCHAR(100) = NULL,
    @UserID BIGINT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Users 
                (FirstName, MiddleName, LastName, DisplayName, 
                Username, Password, UserRole, Email, CreatedBy, UpdatedBy)
    VALUES (@FirstName, @MiddleName, @LastName, @DisplayName, 
                @Username, @Password, @UserRole, @Email, @CreatedBy, @UpdatedBy);

    SET @UserID = SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.sp_UpdateUser', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_UpdateUser;
GO
CREATE PROCEDURE dbo.sp_UpdateUser
    @UserID BIGINT,
    @FirstName NVARCHAR(100),
    @MiddleName NVARCHAR(100) = NULL,
    @LastName NVARCHAR(100),
    @DisplayName NVARCHAR(200) = NULL,
    @Username NVARCHAR(50),
    @Password NVARCHAR(255),
    @UserRole VARCHAR(20),
    @Email NVARCHAR(255),
    @UpdatedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Users
    SET FirstName = @FirstName,
        MiddleName = @MiddleName,
        LastName = @LastName,
        DisplayName = @DisplayName,
        Username = @Username,
        Password = @Password,
        UserRole = @UserRole,
        Email = @Email,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = GETUTCDATE()
    WHERE UserID = @UserID;

    -- Return the updated row
    SELECT UserID, FirstName, MiddleName, LastName, DisplayName, Username, Password,
           UserRole, Email, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy
    FROM Users
    WHERE UserID = @UserID;
END
GO

IF OBJECT_ID('dbo.sp_DeleteUser', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_DeleteUser;
GO
CREATE PROCEDURE dbo.sp_DeleteUser
    @UserID BIGINT,
    @SoftDelete BIT,
    @Deleted BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Users WHERE UserID = @UserID)
    BEGIN
        IF @SoftDelete = 1
        BEGIN
            UPDATE Users
            SET [Status] = 0, UpdatedOn = GETUTCDATE()
            WHERE UserID = @UserID;
        END

        ELSE
        BEGIN
            DELETE FROM Users WHERE UserID = @UserID;
        END

        SET @Deleted = 1;
    END
    ELSE
        SET @Deleted = 0;
END
GO


-- =============================================
-- sp_CheckUsernameExists
-- =============================================
IF OBJECT_ID('dbo.sp_CheckUsernameExists', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CheckUsernameExists;
GO
CREATE PROCEDURE dbo.sp_CheckUsernameExists
    @Username NVARCHAR(50),
    @Exists BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Users WHERE Username = @Username)
        SET @Exists = 1;
    ELSE
        SET @Exists = 0;
END
GO

-- =============================================
-- sp_CheckEmailExists
-- =============================================
IF OBJECT_ID('dbo.sp_CheckEmailExists', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CheckEmailExists;
GO
CREATE PROCEDURE dbo.sp_CheckEmailExists
    @Email NVARCHAR(255),
    @Exists BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email)
        SET @Exists = 1;
    ELSE
        SET @Exists = 0;
END
GO
