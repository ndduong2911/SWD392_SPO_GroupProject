# Hướng dẫn Setup Cloudinary cho Avatar Upload

## Bước 1: Cài đặt NuGet Package

Chạy lệnh sau trong terminal:

```bash
dotnet add SharingPictureOnline package CloudinaryDotNet
```

## Bước 2: Lấy thông tin Cloudinary

1. Đăng nhập vào https://cloudinary.com/console
2. Vào trang Dashboard
3. Copy 3 thông tin sau:
   - **Cloud Name**: dxxxxxxxx
   - **API Key**: 472895543654897 (bạn đã có)
   - **API Secret**: Click vào icon mắt để reveal

## Bước 3: Cập nhật appsettings.json

File `appsettings.json` đã được chuẩn bị sẵn, bạn chỉ cần điền:

```json
"Cloudinary": {
  "CloudName": "YOUR_CLOUD_NAME_HERE",    // <-- Điền Cloud Name
  "ApiKey": "472895543654897",
  "ApiSecret": "YOUR_API_SECRET_HERE"     // <-- Điền API Secret
}
```

## Bước 4: Test

1. Chạy ứng dụng: `dotnet run --project SharingPictureOnline`
2. Đăng nhập và vào Settings
3. Click "Tải ảnh lên" để upload avatar

## Các file đã tạo:

✅ `Services/ICloudinaryService.cs` - Interface
✅ `Services/CloudinaryService.cs` - Implementation
✅ `Components/Pages/Settings.razor` - Updated với upload functionality
✅ `Program.cs` - Registered CloudinaryService
✅ `appsettings.json` - Prepared Cloudinary config

## Lưu ý:

- Avatar tự động resize 500x500px
- Crop theo face detection
- Lưu trong folder "avatars" trên Cloudinary
- Max file size: 5MB
