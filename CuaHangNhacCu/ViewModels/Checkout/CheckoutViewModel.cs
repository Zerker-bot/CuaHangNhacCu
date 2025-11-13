using CuaHangNhacCu.Dto.Cart;
using CuaHangNhacCu.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CuaHangNhacCu.ViewModels.Checkout
{
    public class CheckoutViewModel
    {
        public List<CartItemDto> ItemsToPurchase { get; set; } = new List<CartItemDto>();

        public Address? DefaultAddress { get; set; }
        public Address? TemporaryAddress { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn địa chỉ giao hàng")]
        public int SelectedAddressId { get; set; }

        public decimal Subtotal { get; set; } 
        public decimal ShippingFee { get; set; } 
        public decimal Total { get; set; } 
    }
}
