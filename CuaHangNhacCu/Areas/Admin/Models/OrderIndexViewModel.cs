using CuaHangNhacCu.Models;
using System.Collections.Generic;

namespace CuaHangNhacCu.Areas.Admin.Models
{
    public class OrderIndexViewModel
    {
        public List<Order> Orders { get; set; }

        public OrderStatus CurrentStatus { get; set; }
    }
}
