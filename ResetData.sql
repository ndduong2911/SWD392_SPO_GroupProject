USE SWD392;
GO

-- 1) Xóa TOÀN BỘ thông báo phát sinh (LIKE, COMMENT, FOLLOW, SYSTEM, REPORT)
-- Để làm trống hộp thư thông báo của tất cả người dùng
DELETE FROM [NOTIFICATION];
GO

-- 2) Xóa lịch sử xử lý (AUDIT_LOG) của các báo cáo đã duyệt
-- Tránh việc lưu lại vết xử lý của Admin cũ
DELETE FROM [AUDIT_LOG]
WHERE [reportID] IN (
    SELECT [reportID]
    FROM [REPORT]
    WHERE [status] IN ('RESOLVED', 'REJECTED')
);
GO

-- 3) Đưa các báo cáo đã bị xử lý quay trở về trạng thái đang chờ (PENDING)
UPDATE [REPORT]
SET [status] = 'PENDING'
WHERE [status] IN ('RESOLVED', 'REJECTED');
GO

-- 4) Khôi phục quyền của những tài khoản vô tình bị khóa (BANNED) do kiểm duyệt test
-- Chuyển trạng thái họ quay lại thành USER bình thường
UPDATE [USER]
SET [role] = 'USER'
WHERE [role] = 'BANNED';
GO

-- 5) Kiểm tra nhanh số lượng dữ liệu sau khi reset
SELECT [status], COUNT(*) AS [TotalReports] FROM [REPORT] GROUP BY [status];
SELECT COUNT(*) AS [RemainingNotifications] FROM [NOTIFICATION];
SELECT COUNT(*) AS [RemainingAuditLogs] FROM [AUDIT_LOG];
SELECT COUNT(*) AS [BannedUsers] FROM [USER] WHERE [role] = 'BANNED';
GO