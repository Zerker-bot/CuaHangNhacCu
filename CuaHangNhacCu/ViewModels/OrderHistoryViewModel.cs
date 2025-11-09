using CuaHangNhacCu.Models;

namespace CuaHangNhacCu.ViewModels
{
    public class OrderHistoryViewModel
    {
        public IEnumerable<OrderSummaryDto> Orders { get; set; }

        public OrderStatus? FilterStatus { get; set; }
    }

    public class OrderSummaryDto
    {
        public int OrderId { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public OrderStatus Status { get; set; } 
        public decimal Total { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Discount { get; set; } 

        public ICollection<OrderItemDto> Items { get; set; }
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } 
        public decimal UnitPrice { get; set; } 
        public string ProductName { get; set; } 
    }
}
