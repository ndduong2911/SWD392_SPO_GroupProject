-- =============================================
-- Script: Add DisplayName column to USER_PROFILE
-- =============================================

USE SPO;
GO

-- Check if column exists before adding
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[USER_PROFILE]') AND name = 'displayName')
BEGIN
    ALTER TABLE [USER_PROFILE]
    ADD [displayName] NVARCHAR(255) NULL;
    
    PRINT 'Column [displayName] added successfully to [USER_PROFILE]';
END
ELSE
BEGIN
    PRINT 'Column [displayName] already exists in [USER_PROFILE]';
END
GO
