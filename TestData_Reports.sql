-- =============================================
-- SEED DATA ĐỂ TEST REPORT MANAGEMENT (UC-20)
-- DB: SWD392
-- Password hash: SHA256 -> Base64 (khớp PasswordHasher.cs)
--   123456789 => FeKw08M4keuw8e9gnsQZQgwg4yDOlMZfvIwzEkSOsiU=
-- Tất cả user đều dùng password: 123456789
-- =============================================

USE SWD392;
GO

-- ─────────────────────────────────────────────
-- 1. XÓA DATA CŨ (an toàn, theo thứ tự FK)
-- ─────────────────────────────────────────────
DELETE FROM [REPORT];
DELETE FROM [NOTIFICATION];
DELETE FROM [PHOTO_TAG];
DELETE FROM [ALBUM_PHOTO];
DELETE FROM [LIKE];
DELETE FROM [COMMENT];
DELETE FROM [FOLLOW];
DELETE FROM [PHOTO];
DELETE FROM [ALBUM];
DELETE FROM [TAG];
DELETE FROM [USER_PROFILE];
DELETE FROM [USER];
GO

-- ─────────────────────────────────────────────
-- 2. USERS
-- ─────────────────────────────────────────────
DECLARE @adminId   UNIQUEIDENTIFIER = NEWID();
DECLARE @alexId    UNIQUEIDENTIFIER = NEWID();
DECLARE @linhId    UNIQUEIDENTIFIER = NEWID();
DECLARE @davidId   UNIQUEIDENTIFIER = NEWID();
DECLARE @sarahId   UNIQUEIDENTIFIER = NEWID();
DECLARE @michaelId UNIQUEIDENTIFIER = NEWID();

INSERT INTO [USER] (userID, username, email, passwordHash, role, createdAt) VALUES
(@adminId,   'admin',    'admin@fakepinterest.com',  'FeKw08M4keuw8e9gnsQZQgwg4yDOlMZfvIwzEkSOsiU=', 'ADMIN', GETDATE()),
(@alexId,    'alexnguyen','alex@gmail.com',           'FeKw08M4keuw8e9gnsQZQgwg4yDOlMZfvIwzEkSOsiU=', 'USER',  GETDATE()),
(@linhId,    'linhtran',  'linh@gmail.com',           'FeKw08M4keuw8e9gnsQZQgwg4yDOlMZfvIwzEkSOsiU=', 'USER',  GETDATE()),
(@davidId,   'davidpham', 'david@gmail.com',          'FeKw08M4keuw8e9gnsQZQgwg4yDOlMZfvIwzEkSOsiU=', 'USER',  GETDATE()),
(@sarahId,   'sarahle',   'sarah@gmail.com',          'FeKw08M4keuw8e9gnsQZQgwg4yDOlMZfvIwzEkSOsiU=', 'USER',  GETDATE()),
(@michaelId, 'michaelvu', 'michael@gmail.com',        'FeKw08M4keuw8e9gnsQZQgwg4yDOlMZfvIwzEkSOsiU=', 'USER',  GETDATE());
GO

-- ─────────────────────────────────────────────
-- 3. USER PROFILES
-- ─────────────────────────────────────────────
DECLARE @adminId   UNIQUEIDENTIFIER = (SELECT userID FROM [USER] WHERE username = 'admin');
DECLARE @alexId    UNIQUEIDENTIFIER = (SELECT userID FROM [USER] WHERE username = 'alexnguyen');
DECLARE @linhId    UNIQUEIDENTIFIER = (SELECT userID FROM [USER] WHERE username = 'linhtran');
DECLARE @davidId   UNIQUEIDENTIFIER = (SELECT userID FROM [USER] WHERE username = 'davidpham');
DECLARE @sarahId   UNIQUEIDENTIFIER = (SELECT userID FROM [USER] WHERE username = 'sarahle');
DECLARE @michaelId UNIQUEIDENTIFIER = (SELECT userID FROM [USER] WHERE username = 'michaelvu');

INSERT INTO [USER_PROFILE] (profileID, userID, displayName, bio, avatarURL, website, updatedAt) VALUES
(NEWID(), @adminId,   N'Administrator',    N'Quản trị hệ thống', NULL, NULL, GETDATE()),
(NEWID(), @alexId,    N'Alex Nguyễn',      N'Nhiếp ảnh phong cảnh 🌿', NULL, 'alexphoto.com', GETDATE()),
(NEWID(), @linhId,    N'Linh Trần',        N'Street photographer ☕', NULL, NULL, GETDATE()),
(NEWID(), @davidId,   N'David Phạm',       N'Travel photographer 🇻🇳', NULL, NULL, GETDATE()),
(NEWID(), @sarahId,   N'Sarah Lê',         N'Portrait & lifestyle 📷', NULL, 'sarahle.studio', GETDATE()),
(NEWID(), @michaelId, N'Michael Vũ',       N'Food photographer 🍜', NULL, NULL, GETDATE());
GO

-- ─────────────────────────────────────────────
-- 4. PHOTOS (để có target cho report)
-- ─────────────────────────────────────────────
DECLARE @alexId    UNIQUEIDENTIFIER = (SELECT userID FROM [USER] WHERE username = 'alexnguyen');
DECLARE @davidId   UNIQUEIDENTIFIER = (SELECT userID FROM [USER] WHERE username = 'davidpham');
DECLARE @photo1Id  UNIQUEIDENTIFIER = NEWID();
DECLARE @photo2Id  UNIQUEIDENTIFIER = NEWID();

INSERT INTO [PHOTO] (photoID, userID, title, description, storageKey, visibility, likeCount, commentCount, uploadedAt) VALUES
(@photo1Id, @alexId,  N'Hoàng hôn Hạ Long', N'Ảnh chụp lúc hoàng hôn', 'https://picsum.photos/seed/1/800/600', 'PUBLIC', 12, 3, GETDATE()),
(@photo2Id, @davidId, N'Phố cổ Hội An', N'Đèn lồng đêm rằm', 'https://picsum.photos/seed/2/800/600', 'PUBLIC', 8, 1, GETDATE());
GO

-- ─────────────────────────────────────────────
-- 5. COMMENTS (để có target cho report)
-- ─────────────────────────────────────────────
DECLARE @linhId   UNIQUEIDENTIFIER = (SELECT userID FROM [USER] WHERE username = 'linhtran');
DECLARE @photo1Id UNIQUEIDENTIFIER = (SELECT TOP 1 photoID FROM [PHOTO] WHERE title = N'Hoàng hôn Hạ Long');
DECLARE @comment1Id UNIQUEIDENTIFIER = NEWID();

INSERT INTO [COMMENT] (commentID, photoID, userID, parentID, content, createdAt) VALUES
(@comment1Id, @photo1Id, @linhId, NULL, N'Ảnh đẹp thật! Check out kênh của tao nha http://spam.com', GETDATE());
GO

-- ─────────────────────────────────────────────
-- 6. REPORTS – đủ cases để test UC-20
-- ─────────────────────────────────────────────
DECLARE @adminId    UNIQUEIDENTIFIER = (SELECT userID FROM [USER] WHERE username = 'admin');
DECLARE @alexId     UNIQUEIDENTIFIER = (SELECT userID FROM [USER] WHERE username = 'alexnguyen');
DECLARE @linhId     UNIQUEIDENTIFIER = (SELECT userID FROM [USER] WHERE username = 'linhtran');
DECLARE @davidId    UNIQUEIDENTIFIER = (SELECT userID FROM [USER] WHERE username = 'davidpham');
DECLARE @sarahId    UNIQUEIDENTIFIER = (SELECT userID FROM [USER] WHERE username = 'sarahle');
DECLARE @michaelId  UNIQUEIDENTIFIER = (SELECT userID FROM [USER] WHERE username = 'michaelvu');
DECLARE @photo1Id   UNIQUEIDENTIFIER = (SELECT TOP 1 photoID FROM [PHOTO] WHERE title = N'Hoàng hôn Hạ Long');
DECLARE @photo2Id   UNIQUEIDENTIFIER = (SELECT TOP 1 photoID FROM [PHOTO] WHERE title = N'Phố cổ Hội An');
DECLARE @comment1Id UNIQUEIDENTIFIER = (SELECT TOP 1 commentID FROM [COMMENT]);

-- PENDING: báo cáo ảnh
INSERT INTO [REPORT] (reportID, reporterID, targetType, targetID, reason, status, createdAt) VALUES
(NEWID(), @alexId,   'PHOTO',   @photo2Id,   N'Ảnh chứa nội dung không phù hợp, vi phạm cộng đồng',       'PENDING', GETDATE()),
(NEWID(), @sarahId,  'PHOTO',   @photo1Id,   N'Ảnh vi phạm bản quyền, copy từ nguồn khác không ghi nguồn', 'PENDING', DATEADD(HOUR, -2, GETDATE()));

-- PENDING: báo cáo comment spam
INSERT INTO [REPORT] (reportID, reporterID, targetType, targetID, reason, status, createdAt) VALUES
(NEWID(), @davidId,  'COMMENT', @comment1Id, N'Bình luận spam link quảng cáo, ngôn từ xúc phạm',           'PENDING', DATEADD(HOUR, -5, GETDATE()));

-- PENDING: báo cáo tài khoản
INSERT INTO [REPORT] (reportID, reporterID, targetType, targetID, reason, status, createdAt) VALUES
(NEWID(), @linhId,   'USER',    @michaelId,  N'Tài khoản giả mạo, đăng spam liên tục gây phiền nhiễu',     'PENDING', DATEADD(DAY, -1, GETDATE()));

-- RESOLVED: đã xử lý trước đó (để test tab)
INSERT INTO [REPORT] (reportID, reporterID, targetType, targetID, reason, status, createdAt) VALUES
(NEWID(), @michaelId,'PHOTO',   @photo1Id,   N'Ảnh chứa logo thương mại không được phép',                  'RESOLVED', DATEADD(DAY, -3, GETDATE()));

-- REJECTED: đã bác bỏ (để test tab)
INSERT INTO [REPORT] (reportID, reporterID, targetType, targetID, reason, status, createdAt) VALUES
(NEWID(), @sarahId,  'USER',    @alexId,     N'Tài khoản này đã unfollow tôi nên tôi muốn báo cáo',        'REJECTED', DATEADD(DAY, -2, GETDATE()));
GO

-- ─────────────────────────────────────────────
-- KIỂM TRA
-- ─────────────────────────────────────────────
SELECT 'USERS'     AS [Table], COUNT(*) AS [Count] FROM [USER]
UNION ALL
SELECT 'PHOTOS',   COUNT(*) FROM [PHOTO]
UNION ALL
SELECT 'COMMENTS', COUNT(*) FROM [COMMENT]
UNION ALL
SELECT 'REPORTS',  COUNT(*) FROM [REPORT];

SELECT r.status, COUNT(*) AS cnt FROM [REPORT] r GROUP BY r.status;
GO
