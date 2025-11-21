        using System.ComponentModel.DataAnnotations.Schema;

        using Microsoft.EntityFrameworkCore;

        namespace CuaHangNhacCu.Models;

        public class Order
        {
            public int Id { get; set; }
            public string UserId { get; set; }
            public User User { get; set; }
            public int? ShippingAddressId { get; set; }
            public Address ShippingAddress { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public OrderStatus Status { get; set; } = OrderStatus.Pending;
            [Precision(18, 2)]
            public decimal ShippingFee { get; set; }
            [Precision(18, 2)]
            public decimal Discount { get; set; }
            public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
            [NotMapped]
            public decimal Total
            {
                get
                {
                    decimal itemsTotal = 0;
                    foreach (var i in Items) itemsTotal += i.UnitPrice * i.Quantity;
                    return itemsTotal + ShippingFee - Discount;
                }
            }
        }

         public enum OrderStatus
        {
            Pending,
            Processing,
            Shipped,
            Delivered,
            Cancelled,
            Returned
        }
