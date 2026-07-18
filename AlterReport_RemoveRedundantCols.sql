-- Xóa các cột dư thừa trong REPORT (đã có AUDIT_LOG lưu rồi)
USE SWD392;
GO

-- Xóa FK trước
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Report_ResolvedBy')
    ALTER TABLE [REPORT] DROP CONSTRAINT FK_Report_ResolvedBy;
GO

-- Xóa 3 cột trùng với AUDIT_LOG
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('REPORT') AND name = 'resolvedAction')
    ALTER TABLE [REPORT] DROP COLUMN resolvedAction;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('REPORT') AND name = 'resolvedAt')
    ALTER TABLE [REPORT] DROP COLUMN resolvedAt;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('REPORT') AND name = 'resolvedById')
    ALTER TABLE [REPORT] DROP COLUMN resolvedById;
GO

SELECT 'Redundant columns removed OK' AS Result;
GO
