-- Thêm cột resolvedAction và resolvedAt vào bảng REPORT
USE SWD392;
GO

ALTER TABLE [REPORT]
    ADD [resolvedAction] VARCHAR(50) NULL,
        [resolvedAt]     DATETIME    NULL,
        [resolvedById]   UNIQUEIDENTIFIER NULL;
GO

ALTER TABLE [REPORT]
    ADD CONSTRAINT FK_Report_ResolvedBy
        FOREIGN KEY ([resolvedById]) REFERENCES [USER]([userID]);
GO

SELECT 'ALTER OK' AS Result;
GO
