# Hướng dẫn tạo Database cho FakePinterest

## Bước 1: Tạo Database
Chạy file `Database.sql` để tạo database và các bảng:
```sql
-- Trong SQL Server Management Studio (SSMS):
1. Mở file Database.sql
2. Chọn Execute (F5)
```

## Bước 2: Thêm dữ liệu mẫu
Chạy file `Database_SeedData.sql` để thêm dữ liệu test:
```sql
-- Trong SQL Server Management Studio (SSMS):
1. Mở file Database_SeedData.sql
2. Chọn Execute (F5)
```

## Dữ liệu mẫu bao gồm:
- ✅ **10 Users** với đầy đủ profile
- ✅ **30 Photos** đa dạng chủ đề (Nature, Street, Travel, Portrait, Architecture, Food, Wildlife, Fashion, Minimalism)
- ✅ **10 Albums** phân loại theo chủ đề
- ✅ **20 Tags** phổ biến
- ✅ **90+ Likes** phân bổ cho các photos
- ✅ **7+ Comments** tương tác thực tế
- ✅ **9+ Follows** kết nối giữa users

## Tài khoản test:
| Username | Email | Password | Role |
|----------|-------|----------|------|
| admin | admin@fakepinterest.com | admin123 | Admin |
| alexnguyen | alex.nguyen@gmail.com | password123 | User |
| linhtran | linh.tran@outlook.com | password123 | User |
| davidpham | david.pham@yahoo.com | password123 | User |
| sarahle | sarah.le@hotmail.com | password123 | User |

## Lưu ý:
- Các ảnh sử dụng StorageKey dạng `filename.jpg` (cần upload file thực tế vào thư mục `/uploads/`)
- Password hiện tại là plain text, nên implement hashing trong production
- Connection string mặc định: `Data Source=DESKTOP-BN9D7E3\\SQLEXPRESS;Initial Catalog=SPO;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true`
