
<div align="center">
    <h1>
        🎹 Cửa hàng nhạc cụ
    </h1>
</div>

## Development

1. Clone dự án
2. Copy `appsettings.Example.json` thành `appsettings.json`
3. Điền các biến môi trường
4. Chạy lệnh
```bash
dotnet ef database update
```

### Tạo tài khoản Admin

1. Đảm bảo đã điền thông tin Admin trong file `appsettings.json`
2. Chạy lệnh
```bash
dotnet run -- seed-admin
```

### Tạo dữ liệu mẫu

```bash
dotnet run -- seed-data
```

