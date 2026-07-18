-- Tạo bảng AUDIT_LOG để ghi lại mọi hành động xử lý report
USE SWD392;
GO

CREATE TABLE [AUDIT_LOG] (
    [logID]       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [reportID]    UNIQUEIDENTIFIER NOT NULL,
    [actorID]     UNIQUEIDENTIFIER NOT NULL,
    [action]      VARCHAR(50)      NOT NULL,
    [note]        NVARCHAR(MAX)    NULL,
    [createdAt]   DATETIME         NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_AuditLog PRIMARY KEY ([logID]),
    CONSTRAINT FK_AuditLog_Report FOREIGN KEY ([reportID]) REFERENCES [REPORT]([reportID]) ON DELETE CASCADE,
    CONSTRAINT FK_AuditLog_Actor  FOREIGN KEY ([actorID])  REFERENCES [USER]([userID])
);
GO

SELECT 'AUDIT_LOG created OK' AS Result;
GO
