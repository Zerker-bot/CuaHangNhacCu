<<<<<<< HEAD
        using System.ComponentModel.DataAnnotations.Schema;
=======
﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
>>>>>>> master

        using Microsoft.EntityFrameworkCore;

        namespace CuaHangNhacCu.Models;

<<<<<<< HEAD
        public class Order
=======
public class Order
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public User? User { get; set; }
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
>>>>>>> master
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

<<<<<<< HEAD
         public enum OrderStatus
        {
            Pending,
            Processing,
            Shipped,
            Delivered,
            Cancelled,
            Returned
        }
=======
 public enum OrderStatus
{

    [Display(Name = "Chờ xử lý")]
    Pending,
    [Display(Name = "Đang xử lý")]
    Processing,
    [Display(Name = "Đang vận chuyển")]
    Shipped,
    [Display(Name = "Đã nhận hàng")]
    Delivered,
    [Display(Name = "Đã huỷ")]
    Cancelled,
    [Display(Name = "Đã trả hàng")]
    Returned
}
>>>>>>> master
