-- =============================================
-- Script: Seed Data cho FakePinterest Database
-- Mô tả: Tạo dữ liệu mẫu cho hệ thống chia sẻ ảnh
-- =============================================

USE SPO;
GO

-- Xóa dữ liệu cũ (nếu có)
DELETE FROM [PHOTO_TAG];
DELETE FROM [ALBUM_PHOTO];
DELETE FROM [NOTIFICATION];
DELETE FROM [REPORT];
DELETE FROM [LIKE];
DELETE FROM [COMMENT];
DELETE FROM [FOLLOW];
DELETE FROM [PHOTO];
DELETE FROM [ALBUM];
DELETE FROM [TAG];
DELETE FROM [USER_PROFILE];
DELETE FROM [USER];
GO

-- =============================================
-- 1. THÊM USERS (10 người dùng)
-- =============================================
-- Password hash sử dụng SHA256 (tương ứng với PasswordHasher.cs)
INSERT INTO [USER] ([userID], [username], [email], [passwordHash], [role], [createdAt]) VALUES
(NEWID(), 'admin', 'admin@fakepinterest.com', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'ADMIN', GETDATE()),
(NEWID(), 'alexnguyen', 'alex.nguyen@gmail.com', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 'USER', GETDATE()),
(NEWID(), 'linhtran', 'linh.tran@outlook.com', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 'USER', GETDATE()),
(NEWID(), 'davidpham', 'david.pham@yahoo.com', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 'USER', GETDATE()),
(NEWID(), 'sarahle', 'sarah.le@hotmail.com', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 'USER', GETDATE()),
(NEWID(), 'michaelvu', 'michael.vu@gmail.com', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 'USER', GETDATE()),
(NEWID(), 'jennyngo', 'jenny.ngo@gmail.com', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 'USER', GETDATE()),
(NEWID(), 'roberthoang', 'robert.hoang@outlook.com', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 'USER', GETDATE()),
(NEWID(), 'emilydang', 'emily.dang@yahoo.com', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 'USER', GETDATE()),
(NEWID(), 'johndo', 'john.do@gmail.com', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 'USER', GETDATE());
GO

-- =============================================
-- 2. THÊM USER PROFILES
-- =============================================
DECLARE @adminId UNIQUEIDENTIFIER = (SELECT [userID] FROM [USER] WHERE [username] = 'admin');
DECLARE @alexId UNIQUEIDENTIFIER = (SELECT [userID] FROM [USER] WHERE [username] = 'alexnguyen');
DECLARE @linhId UNIQUEIDENTIFIER = (SELECT [userID] FROM [USER] WHERE [username] = 'linhtran');
DECLARE @davidId UNIQUEIDENTIFIER = (SELECT [userID] FROM [USER] WHERE [username] = 'davidpham');
DECLARE @sarahId UNIQUEIDENTIFIER = (SELECT [userID] FROM [USER] WHERE [username] = 'sarahle');
DECLARE @michaelId UNIQUEIDENTIFIER = (SELECT [userID] FROM [USER] WHERE [username] = 'michaelvu');
DECLARE @jennyId UNIQUEIDENTIFIER = (SELECT [userID] FROM [USER] WHERE [username] = 'jennyngo');
DECLARE @robertId UNIQUEIDENTIFIER = (SELECT [userID] FROM [USER] WHERE [username] = 'roberthoang');
DECLARE @emilyId UNIQUEIDENTIFIER = (SELECT [userID] FROM [USER] WHERE [username] = 'emilydang');
DECLARE @johnId UNIQUEIDENTIFIER = (SELECT [userID] FROM [USER] WHERE [username] = 'johndo');

INSERT INTO [USER_PROFILE] ([profileID], [userID], [bio], [avatarURL], [website], [updatedAt]) VALUES
(NEWID(), @adminId, N'System Administrator', NULL, NULL, GETDATE()),
(NEWID(), @alexId, N'Nhiếp ảnh phong cảnh | Nature lover 🌿', NULL, 'alexphoto.com', GETDATE()),
(NEWID(), @linhId, N'Street photographer | Coffee addict ☕', NULL, 'linhtran.photography', GETDATE()),
(NEWID(), @davidId, N'Travel photographer exploring Vietnam 🇻🇳', NULL, NULL, GETDATE()),
(NEWID(), @sarahId, N'Portrait & lifestyle photographer 📷', NULL, 'sarahle.studio', GETDATE()),
(NEWID(), @michaelId, N'Architectural photography enthusiast 🏛️', NULL, NULL, GETDATE()),
(NEWID(), @jennyId, N'Food & product photographer 🍜', NULL, 'jennyngo.photo', GETDATE()),
(NEWID(), @robertId, N'Wildlife & nature photography 🦅', NULL, NULL, GETDATE()),
(NEWID(), @emilyId, N'Fashion & beauty photographer ✨', NULL, 'emilydang.vn', GETDATE()),
(NEWID(), @johnId, N'Minimalist photography lover 🎨', NULL, NULL, GETDATE());
GO

-- =============================================
-- 3. THÊM TAGS (20 tags phổ biến)
-- =============================================
INSERT INTO [TAG] ([tagID], [name], [usageCount]) VALUES
(NEWID(), N'Nature', 0),
(NEWID(), N'Cyberpunk', 0),
(NEWID(), N'Minimalism', 0),
(NEWID(), N'StreetPhotography', 0),
(NEWID(), N'Travel2026', 0),
(NEWID(), N'Portrait', 0),
(NEWID(), N'Architecture', 0),
(NEWID(), N'Sunset', 0),
(NEWID(), N'Wildlife', 0),
(NEWID(), N'Food', 0),
(NEWID(), N'Fashion', 0),
(NEWID(), N'BlackAndWhite', 0),
(NEWID(), N'Landscape', 0),
(NEWID(), N'CityLife', 0),
(NEWID(), N'Beach', 0),
(NEWID(), N'Mountain', 0),
(NEWID(), N'Coffee', 0),
(NEWID(), N'Art', 0),
(NEWID(), N'Photography', 0),
(NEWID(), N'Vietnam', 0);
GO

-- =============================================
-- 4. THÊM PHOTOS (30 ảnh mẫu)
-- =============================================
INSERT INTO [PHOTO] ([photoID], [userID], [title], [description], [storageKey], [visibility], [likeCount], [commentCount], [uploadedAt]) VALUES
-- Alex's Photos (Nature)
(NEWID(), @alexId, N'Bình minh trên biển', N'Khoảnh khắc bình minh tuyệt đẹp tại bãi biển Mỹ Khê', 'https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=500', 'PUBLIC', 156, 23, DATEADD(DAY, -10, GETDATE())),
(NEWID(), @alexId, N'Núi rừng Tây Bắc', N'Sương mù bao phủ núi rừng Sa Pa', 'https://images.unsplash.com/photo-1470071459604-3b5ec3a7fe05?w=500', 'PUBLIC', 234, 45, DATEADD(DAY, -8, GETDATE())),
(NEWID(), @alexId, N'Thác nước hùng vĩ', N'Thác Bản Giốc mùa nước lũ', 'https://images.unsplash.com/photo-1433086966358-54859d0ed716?w=500', 'PUBLIC', 189, 32, DATEADD(DAY, -5, GETDATE())),

-- Linh's Photos (Street)
(NEWID(), @linhId, N'Phố cổ Hà Nội', N'Góc nhỏ yên bình giữa phố cổ', 'https://images.unsplash.com/photo-1542038784456-1ea8e935640e?w=500', 'PUBLIC', 298, 67, DATEADD(DAY, -12, GETDATE())),
(NEWID(), @linhId, N'Xe ôm Sài Gòn', N'Cuộc sống thường ngày ở Sài Gòn', 'https://images.unsplash.com/photo-1583417319070-4a69db38a482?w=500', 'PUBLIC', 412, 89, DATEADD(DAY, -9, GETDATE())),
(NEWID(), @linhId, N'Cafe vỉa hè', N'Văn hóa cafe vỉa hè Việt Nam', 'https://images.unsplash.com/photo-1511920170033-f8396924c348?w=500', 'PUBLIC', 345, 72, DATEADD(DAY, -4, GETDATE())),

-- David's Photos (Travel)
(NEWID(), @davidId, N'Vịnh Hạ Long', N'Di sản thiên nhiên thế giới', 'https://images.unsplash.com/photo-1528127269322-539801943592?w=500', 'PUBLIC', 567, 123, DATEADD(DAY, -15, GETDATE())),
(NEWID(), @davidId, N'Phố cổ Hội An', N'Đêm lồng đèn Hội An', 'https://images.unsplash.com/photo-1555217851-5af58aa8d8f8?w=500', 'PUBLIC', 498, 98, DATEADD(DAY, -11, GETDATE())),
(NEWID(), @davidId, N'Đèo Hải Vân', N'Con đường ven biển đẹp nhất Việt Nam', 'https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=500', 'PUBLIC', 423, 87, DATEADD(DAY, -6, GETDATE())),

-- Sarah's Photos (Portrait)
(NEWID(), @sarahId, N'Áo dài truyền thống', N'Vẻ đẹp của tà áo dài Việt', 'https://images.unsplash.com/photo-1617391551938-3621a3c3e2c7?w=500', 'PUBLIC', 389, 91, DATEADD(DAY, -14, GETDATE())),
(NEWID(), @sarahId, N'Nụ cười trẻ thơ', N'Em bé vùng cao', 'https://images.unsplash.com/photo-1503454537195-1dcabb73ffb9?w=500', 'PUBLIC', 512, 105, DATEADD(DAY, -10, GETDATE())),
(NEWID(), @sarahId, N'Chân dung nghệ nhân', N'Nghệ nhân làng gốm Bát Tràng', 'https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=500', 'PUBLIC', 267, 56, DATEADD(DAY, -3, GETDATE())),

-- Michael's Photos (Architecture)
(NEWID(), @michaelId, N'Landmark 81', N'Tòa nhà cao nhất Việt Nam', 'https://images.unsplash.com/photo-1480714378408-67cf0d13bc1b?w=500', 'PUBLIC', 445, 78, DATEADD(DAY, -13, GETDATE())),
(NEWID(), @michaelId, N'Nhà thờ Đức Bà', N'Kiến trúc Gothic cổ điển', 'https://images.unsplash.com/photo-1547036967-23d11aacaee0?w=500', 'PUBLIC', 378, 69, DATEADD(DAY, -7, GETDATE())),
(NEWID(), @michaelId, N'Cầu Vàng Đà Nẵng', N'Bàn tay khổng lồ nâng cây cầu', 'https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=500', 'PUBLIC', 689, 142, DATEADD(DAY, -2, GETDATE())),

-- Jenny's Photos (Food)
(NEWID(), @jennyId, N'Phở Việt Nam', N'Món ăn quốc hồn quốc tuý', 'https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=500', 'PUBLIC', 523, 98, DATEADD(DAY, -16, GETDATE())),
(NEWID(), @jennyId, N'Bánh mì Sài Gòn', N'Bánh mì thịt nướng đặc sản', 'https://images.unsplash.com/photo-1603133872878-684f208fb84b?w=500', 'PUBLIC', 467, 87, DATEADD(DAY, -8, GETDATE())),
(NEWID(), @jennyId, N'Cà phê sữa đá', N'Hương vị cà phê Việt', 'https://images.unsplash.com/photo-1509042239860-f550ce710b93?w=500', 'PUBLIC', 398, 71, DATEADD(DAY, -1, GETDATE())),

-- Robert's Photos (Wildlife)
(NEWID(), @robertId, N'Chim hồng hạc', N'Đàn chim tại Cát Bà', 'https://images.unsplash.com/photo-1535083783855-76ae62b2914e?w=500', 'PUBLIC', 312, 54, DATEADD(DAY, -17, GETDATE())),
(NEWID(), @robertId, N'Khỉ vàng Cát Bà', N'Loài vật quý hiếm', 'https://images.unsplash.com/photo-1540573133985-87b6da6d54a9?w=500', 'PUBLIC', 289, 48, DATEADD(DAY, -9, GETDATE())),
(NEWID(), @robertId, N'Rừng ngập mặn', N'Hệ sinh thái Cần Giờ', 'https://images.unsplash.com/photo-1441974231531-c6227db76b6e?w=500', 'PUBLIC', 234, 41, DATEADD(DAY, -4, GETDATE())),

-- Emily's Photos (Fashion)
(NEWID(), @emilyId, N'Fashion Week 2026', N'Sàn diễn thời trang Việt Nam', 'https://images.unsplash.com/photo-1485230895905-ec40ba36b9bc?w=500', 'PUBLIC', 612, 115, DATEADD(DAY, -18, GETDATE())),
(NEWID(), @emilyId, N'Street Style', N'Phong cách đường phố giới trẻ', 'https://images.unsplash.com/photo-1483985988355-763728e1935b?w=500', 'PUBLIC', 534, 96, DATEADD(DAY, -11, GETDATE())),
(NEWID(), @emilyId, N'Traditional meets Modern', N'Áo dài cách tân', 'https://images.unsplash.com/photo-1539533018447-63fcce2678e3?w=500', 'PUBLIC', 478, 89, DATEADD(DAY, -5, GETDATE())),

-- John's Photos (Minimalism)
(NEWID(), @johnId, N'Bình minh tối giản', N'Một mình trên biển', 'https://images.unsplash.com/photo-1502082553048-f009c37129b9?w=500', 'PUBLIC', 356, 63, DATEADD(DAY, -19, GETDATE())),
(NEWID(), @johnId, N'Kiến trúc tối giản', N'Đường nét đơn giản', 'https://images.unsplash.com/photo-1513694203232-719a280e022f?w=500', 'PUBLIC', 423, 78, DATEADD(DAY, -12, GETDATE())),
(NEWID(), @johnId, N'Không gian trống', N'Vẻ đẹp của sự đơn giản', 'https://images.unsplash.com/photo-1487260211189-670c54da558d?w=500', 'PUBLIC', 289, 52, DATEADD(DAY, -6, GETDATE())),

-- Admin's Photos
(NEWID(), @adminId, N'Cờ Việt Nam', N'Lá cờ đỏ sao vàng', 'https://images.unsplash.com/photo-1583417319070-4a69db38a482?w=500', 'PUBLIC', 789, 156, DATEADD(DAY, -20, GETDATE())),
(NEWID(), @adminId, N'Hồ Hoàn Kiếm', N'Trái tim của Hà Nội', 'https://images.unsplash.com/photo-1528127269322-539801943592?w=500', 'PUBLIC', 678, 134, DATEADD(DAY, -15, GETDATE())),
(NEWID(), @adminId, N'Sài Gòn về đêm', N'Thành phố không ngủ', 'https://images.unsplash.com/photo-1514565131-fce0801e5785?w=500', 'PUBLIC', 845, 167, DATEADD(DAY, -7, GETDATE()));
GO

-- =============================================
-- 5. THÊM ALBUMS (10 albums)
-- =============================================
INSERT INTO [ALBUM] ([albumID], [userID], [title], [coverPhotoID], [visibility], [createdAt]) VALUES
(NEWID(), @alexId, N'Khám phá Việt Nam', NULL, 'PUBLIC', DATEADD(DAY, -20, GETDATE())),
(NEWID(), @linhId, N'Street Life Collection', NULL, 'PUBLIC', DATEADD(DAY, -18, GETDATE())),
(NEWID(), @davidId, N'Travel Vietnam 2026', NULL, 'PUBLIC', DATEADD(DAY, -15, GETDATE())),
(NEWID(), @sarahId, N'Portrait Series', NULL, 'PUBLIC', DATEADD(DAY, -12, GETDATE())),
(NEWID(), @michaelId, N'Modern Architecture', NULL, 'PUBLIC', DATEADD(DAY, -10, GETDATE())),
(NEWID(), @jennyId, N'Vietnamese Cuisine', NULL, 'PUBLIC', DATEADD(DAY, -8, GETDATE())),
(NEWID(), @robertId, N'Wildlife Vietnam', NULL, 'PUBLIC', DATEADD(DAY, -6, GETDATE())),
(NEWID(), @emilyId, N'Fashion Photography', NULL, 'PUBLIC', DATEADD(DAY, -4, GETDATE())),
(NEWID(), @johnId, N'Minimalist Art', NULL, 'PUBLIC', DATEADD(DAY, -2, GETDATE())),
(NEWID(), @adminId, N'Best of Vietnam', NULL, 'PUBLIC', DATEADD(DAY, -25, GETDATE()));
GO

-- =============================================
-- 6. THÊM LIKES (Random likes cho photos)
-- =============================================
DECLARE @photoId UNIQUEIDENTIFIER;
DECLARE @counter INT = 0;

DECLARE photo_cursor CURSOR FOR SELECT [photoID] FROM [PHOTO];
OPEN photo_cursor;
FETCH NEXT FROM photo_cursor INTO @photoId;

WHILE @@FETCH_STATUS = 0 AND @counter < 10
BEGIN
    INSERT INTO [LIKE] ([likeID], [userID], [photoID], [createdAt]) VALUES
    (NEWID(), @alexId, @photoId, DATEADD(HOUR, -CAST((RAND() * 100) AS INT), GETDATE())),
    (NEWID(), @linhId, @photoId, DATEADD(HOUR, -CAST((RAND() * 100) AS INT), GETDATE())),
    (NEWID(), @davidId, @photoId, DATEADD(HOUR, -CAST((RAND() * 100) AS INT), GETDATE()));
    
    SET @counter = @counter + 1;
    FETCH NEXT FROM photo_cursor INTO @photoId;
END;

CLOSE photo_cursor;
DEALLOCATE photo_cursor;
GO

-- =============================================
-- 7. THÊM COMMENTS (30 comments mẫu)
-- =============================================
DECLARE @photo1 UNIQUEIDENTIFIER = (SELECT TOP 1 [photoID] FROM [PHOTO] ORDER BY [uploadedAt] DESC);
DECLARE @photo2 UNIQUEIDENTIFIER = (SELECT TOP 1 [photoID] FROM [PHOTO] WHERE [title] LIKE N'%Vịnh Hạ Long%');
DECLARE @photo3 UNIQUEIDENTIFIER = (SELECT TOP 1 [photoID] FROM [PHOTO] WHERE [title] LIKE N'%Cầu Vàng%');

INSERT INTO [COMMENT] ([commentID], [photoID], [userID], [parentID], [content], [createdAt]) VALUES
(NEWID(), @photo1, @linhId, NULL, N'Ảnh đẹp quá! 😍', DATEADD(HOUR, -5, GETDATE())),
(NEWID(), @photo1, @davidId, NULL, N'Góc chụp tuyệt vời!', DATEADD(HOUR, -4, GETDATE())),
(NEWID(), @photo1, @sarahId, NULL, N'Love it! 💕', DATEADD(HOUR, -3, GETDATE())),
(NEWID(), @photo2, @michaelId, NULL, N'Vịnh Hạ Long đẹp nhất Việt Nam!', DATEADD(HOUR, -10, GETDATE())),
(NEWID(), @photo2, @jennyId, NULL, N'Mình cũng vừa đi đây tuần trước', DATEADD(HOUR, -9, GETDATE())),
(NEWID(), @photo3, @robertId, NULL, N'Cầu Vàng nổi tiếng thế giới!', DATEADD(HOUR, -7, GETDATE())),
(NEWID(), @photo3, @emilyId, NULL, N'Amazing shot! 🌟', DATEADD(HOUR, -6, GETDATE()));
GO

-- =============================================
-- 8. THÊM FOLLOWS (User theo dõi lẫn nhau)
-- =============================================
INSERT INTO [FOLLOW] ([followID], [followerID], [followingID], [followedAt]) VALUES
-- Alex follows
(NEWID(), @alexId, @linhId, DATEADD(DAY, -30, GETDATE())),
(NEWID(), @alexId, @davidId, DATEADD(DAY, -28, GETDATE())),
(NEWID(), @alexId, @sarahId, DATEADD(DAY, -25, GETDATE())),
-- Linh follows
(NEWID(), @linhId, @alexId, DATEADD(DAY, -29, GETDATE())),
(NEWID(), @linhId, @davidId, DATEADD(DAY, -27, GETDATE())),
(NEWID(), @linhId, @michaelId, DATEADD(DAY, -24, GETDATE())),
-- David follows
(NEWID(), @davidId, @alexId, DATEADD(DAY, -26, GETDATE())),
(NEWID(), @davidId, @linhId, DATEADD(DAY, -23, GETDATE())),
(NEWID(), @davidId, @jennyId, DATEADD(DAY, -20, GETDATE()));
GO

-- =============================================
-- 9. CẬP NHẬT USAGE COUNT CHO TAGS
-- =============================================
UPDATE [TAG] SET [usageCount] = 15 WHERE [name] = N'Nature';
UPDATE [TAG] SET [usageCount] = 8 WHERE [name] = N'Travel2026';
UPDATE [TAG] SET [usageCount] = 12 WHERE [name] = N'Portrait';
UPDATE [TAG] SET [usageCount] = 10 WHERE [name] = N'Architecture';
UPDATE [TAG] SET [usageCount] = 7 WHERE [name] = N'Food';
UPDATE [TAG] SET [usageCount] = 6 WHERE [name] = N'Minimalism';
UPDATE [TAG] SET [usageCount] = 9 WHERE [name] = N'StreetPhotography';
UPDATE [TAG] SET [usageCount] = 14 WHERE [name] = N'Vietnam';
GO

PRINT '========================================';
PRINT 'SEED DATA COMPLETED SUCCESSFULLY!';
PRINT '========================================';
PRINT 'Users: 10';
PRINT 'Photos: 30';
PRINT 'Albums: 10';
PRINT 'Tags: 20';
PRINT 'Likes: 30+';
PRINT 'Comments: 7+';
PRINT 'Follows: 9+';
PRINT '========================================';
PRINT '';
PRINT 'TEST ACCOUNTS:';
PRINT 'Admin: admin / admin123';
PRINT 'User: alexnguyen / password123';
PRINT 'User: linhtran / password123';
PRINT '========================================';
GO
