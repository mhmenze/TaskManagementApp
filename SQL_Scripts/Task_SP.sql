-- =============================================
-- Author:        Nazeer
-- Create date:   2025-11-09
-- Description:   Stored Procedures for Task Management
-- =============================================

-- =============================================
-- sp_GetAllTasks
-- =============================================
IF OBJECT_ID('dbo.sp_GetAllTasks', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetAllTasks;
GO
CREATE PROCEDURE dbo.sp_GetAllTasks
    @Status INT = NULL,
    @AssignedUserID BIGINT = NULL,
    @IsDelayed BIT = NULL,
    @SearchTerm NVARCHAR(200) = NULL,
    @SortBy NVARCHAR(50) = NULL,
    @SortDescending BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TaskID, TaskName, TaskDescription, AssignedUserIDs, Status, IsDelayed,
           Deadline, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy
    FROM Tasks
    WHERE (@Status IS NULL OR Status = @Status)
      AND (@AssignedUserID IS NULL OR AssignedUserIDs LIKE '%' + CAST(@AssignedUserID AS NVARCHAR) + '%')
      AND (@IsDelayed IS NULL OR IsDelayed = @IsDelayed)
      AND (@SearchTerm IS NULL OR TaskName LIKE '%' + @SearchTerm + '%' OR TaskDescription LIKE '%' + @SearchTerm + '%')
    ORDER BY 
        CASE WHEN @SortBy = 'name' AND @SortDescending = 0 THEN TaskName END ASC,
        CASE WHEN @SortBy = 'name' AND @SortDescending = 1 THEN TaskName END DESC,
        CASE WHEN @SortBy = 'deadline' AND @SortDescending = 0 THEN Deadline END ASC,
        CASE WHEN @SortBy = 'deadline' AND @SortDescending = 1 THEN Deadline END DESC,
        CASE WHEN @SortBy = 'status' AND @SortDescending = 0 THEN Status END ASC,
        CASE WHEN @SortBy = 'status' AND @SortDescending = 1 THEN Status END DESC,
        CASE WHEN @SortBy = 'created' AND @SortDescending = 0 THEN CreatedOn END ASC,
        CASE WHEN @SortBy = 'created' AND @SortDescending = 1 THEN CreatedOn END DESC,
        CASE WHEN @SortBy IS NULL THEN TaskID END DESC;
END
GO

-- =============================================
-- sp_GetTaskByUserID
-- =============================================
IF OBJECT_ID('dbo.sp_GetTasksByUserID', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetTasksByUserID;
GO
CREATE PROCEDURE dbo.sp_GetTasksByUserID
    @UserID BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TaskID, TaskName, TaskDescription, AssignedUserIDs, Status, IsDelayed,
           Deadline, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy
    FROM Tasks
    WHERE EXISTS (
        SELECT 1 
        FROM OPENJSON(AssignedUserIDs)
        WHERE value = CAST(@UserID AS NVARCHAR(20))
    );
END
GO


-- =============================================
-- sp_GetTaskById
-- =============================================
IF OBJECT_ID('dbo.sp_GetTaskById', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetTaskById;
GO
CREATE PROCEDURE dbo.sp_GetTaskById
    @TaskID BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TaskID, TaskName, TaskDescription, AssignedUserIDs, Status, IsDelayed,
           Deadline, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy
    FROM Tasks
    WHERE TaskID = @TaskID;
        --AND Status <> 4;
END
GO

-- =============================================
-- sp_CreateTask
-- =============================================
IF OBJECT_ID('dbo.sp_CreateTask', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CreateTask;
GO
CREATE PROCEDURE dbo.sp_CreateTask
    @TaskName NVARCHAR(200),
    @TaskDescription NVARCHAR(1000) = NULL,
    @AssignedUserIDs NVARCHAR(MAX) = NULL,
    @Status INT = 0,
    @IsDelayed BIT = 0,
    @Deadline DATETIME = NULL,
    @CreatedBy NVARCHAR(100) = NULL,
    @UpdatedBy NVARCHAR(100) = NULL,
    @TaskID BIGINT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Tasks (TaskName, TaskDescription, AssignedUserIDs, Status, IsDelayed, Deadline, CreatedBy, UpdatedBy)
    VALUES (@TaskName, @TaskDescription, @AssignedUserIDs, @Status, @IsDelayed, @Deadline, @CreatedBy, @UpdatedBy);

    SET @TaskID = SCOPE_IDENTITY();
END
GO

-- =============================================
-- sp_UpdateTask
-- =============================================
IF OBJECT_ID('dbo.sp_UpdateTask', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_UpdateTask;
GO
CREATE PROCEDURE dbo.sp_UpdateTask
    @TaskID BIGINT,
    @TaskName NVARCHAR(200),
    @TaskDescription NVARCHAR(1000) = NULL,
    @AssignedUserIDs NVARCHAR(MAX) = NULL,
    @Status INT,
    @IsDelayed BIT,
    @Deadline DATETIME = NULL,
    @UpdatedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Tasks
    SET TaskName = @TaskName,
        TaskDescription = @TaskDescription,
        AssignedUserIDs = @AssignedUserIDs,
        Status = @Status,
        IsDelayed = @IsDelayed,
        Deadline = @Deadline,
        UpdatedOn = GETUTCDATE(),
        UpdatedBy = @UpdatedBy
    WHERE TaskID = @TaskID;
END
GO

-- =============================================
-- sp_UpdateTaskStatus
-- =============================================
IF OBJECT_ID('dbo.sp_UpdateTaskStatus', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_UpdateTaskStatus;
GO
CREATE PROCEDURE dbo.sp_UpdateTaskStatus
    @TaskID BIGINT,
    @Status INT,
    @UpdatedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Tasks
    SET Status = @Status,
        UpdatedOn = GETUTCDATE(),
        UpdatedBy = @UpdatedBy
    WHERE TaskID = @TaskID;
END
GO

-- =============================================
-- sp_DeleteTask
-- =============================================
IF OBJECT_ID('dbo.sp_DeleteTask', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_DeleteTask;
GO
CREATE PROCEDURE dbo.sp_DeleteTask
    @TaskID BIGINT,
    @DeletedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Tasks
    SET Status = 4,
        UpdatedOn = GETUTCDATE(),
        UpdatedBy = @DeletedBy
    WHERE TaskID = @TaskID;
END
GO
