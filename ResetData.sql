USE SWD392;
GO

-- 1) Xóa TOÀN BỘ thông báo để mục thông báo của người dùng trống
--    (không chỉ type REPORT — gồm LIKE, COMMENT, FOLLOW, SYSTEM, REPORT)
--    refID không có FK tới REPORT nên xóa tự do
DELETE FROM [NOTIFICATION];
GO

-- 2) Xóa lịch sử xử lý (AUDIT_LOG) của các report đã xử lý
--    để khi đưa về PENDING không còn dấu đã xử lý
DELETE FROM [AUDIT_LOG]
WHERE [reportID] IN (
    SELECT [reportID]
    FROM [REPORT]
    WHERE [status] IN ('RESOLVED', 'REJECTED')
);
GO

-- 3) Đưa các report đã xử lý về trạng thái đang chờ xử lý (PENDING)
UPDATE [REPORT]
SET [status] = 'PENDING'
WHERE [status] IN ('RESOLVED', 'REJECTED');
GO

-- Kiểm tra nhanh (tuỳ chọn)
SELECT [status], COUNT(*) AS [cnt]
FROM [REPORT]
GROUP BY [status];

SELECT COUNT(*) AS [notificationCount] FROM [NOTIFICATION];

SELECT COUNT(*) AS [auditLogCount] FROM [AUDIT_LOG];
GO