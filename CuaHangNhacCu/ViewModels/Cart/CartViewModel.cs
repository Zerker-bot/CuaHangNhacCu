using CuaHangNhacCu.Dto.Cart;
using System.Collections.Generic;
using System.Linq;

namespace CuaHangNhacCu.ViewModels.Cart
{
    public class CartViewModel
    {
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public decimal TotalPrice => Items.Sum(item => item.TotalItemPrice);
    }
}
