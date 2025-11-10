-- =============================================
-- Insert Dummy Data
-- =============================================

-- Insert Sample Users
INSERT INTO Users (FirstName, LastName, Username, Password, Email, DisplayName, CreatedBy, UpdatedBy)
VALUES 
    ('John', 'Doe', 'johndoe', 'password123', 'john.doe@example.com', 'John Doe', 'System', 'System'),
    ('Jane', 'Smith', 'janesmith', 'password123', 'jane.smith@example.com', 'Jane Smith', 'System', 'System'),
    ('Admin', 'User', 'admin', 'admin123', 'admin@example.com', 'Administrator', 'System', 'System');
GO

-- Insert Sample Tasks
INSERT INTO Tasks (TaskName, TaskDescription, AssignedUserIDs, Status, IsDelayed, Deadline, CreatedBy, UpdatedBy)
VALUES 
    ('Complete Project Documentation', 'Write comprehensive documentation for the project', '[1,2]', 0, 0, DATEADD(day, 7, GETUTCDATE()), 'admin', 'admin'),
    ('Review Code Changes', 'Review pull requests and provide feedback', '[2]', 1, 0, DATEADD(day, 2, GETUTCDATE()), 'admin', 'admin'),
    ('Deploy to Production', 'Deploy the latest version to production environment', '[1]', 0, 0, DATEADD(day, 14, GETUTCDATE()), 'admin', 'admin');
GO