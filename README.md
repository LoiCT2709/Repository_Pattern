# Shop Application (.NET Console App)

Ứng dụng mẫu mô phỏng hệ thống bán hàng đơn giản sử dụng kiến trúc **Repository Pattern** và **Unit of Work**. Hỗ trợ thao tác với đơn hàng (Order), sản phẩm (Product), sử dụng cơ sở dữ liệu SQL Server và In-Memory.

## Kiến trúc sử dụng
- Repository Pattern
- Unit of Work Pattern
- Entity Model (POCO)
- SQL Server / In-Memory Repository

## Các thành phần chính

| Thư mục | Mô tả |
|--------|------|
| `Entities/` | Các lớp thực thể: `Product`, `Order`, `OrderItem` |
| `Repository/` | Interface & implementation cho Repository |
| `UnitOfWork/` | Lớp quản lý giao dịch `CheckoutUnitOfWork` |
| `Program.cs` | Hàm `Main` – nơi chạy chương trình chính |

## 🗂 Cấu trúc Entity

### Product
- `Id`: Guid
- `Name`: string
- `Price`: double
- `Quantity`: int

### Order
- `Id`: Guid
- `OrderReference`: string
- `CustomerId`: Guid
- `Items`: List\<OrderItem>

### OrderItem
- `Id`: Guid
- `ProductId`: Guid
- `Quantity`: int
- `Price`: double

---

##  Cách chạy chương trình

### 1. Cấu hình chuỗi kết nối

Cập nhật chuỗi kết nối SQL Server trong `appsettings.json` hoặc file config:

```json
{
  "ConnectionString": {
    "Default": "Server=YOUR_SERVER;Database=Shop;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
