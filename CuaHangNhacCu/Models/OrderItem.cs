using Microsoft.EntityFrameworkCore;

namespace CuaHangNhacCu.Models;

public class OrderItem
{
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
    [Precision(18, 2)]
    public decimal UnitPrice { get; set; }
}
