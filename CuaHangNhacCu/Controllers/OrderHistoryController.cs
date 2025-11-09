using CuaHangNhacCu.Data;
using CuaHangNhacCu.Models;
using CuaHangNhacCu.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace CuaHangNhacCu.Controllers

{
    public class OrderHistoryController : Controller
    {


        private readonly ApplicationDbContext _context;

        public OrderHistoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(Models.OrderStatus? FilterStatus)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            var  ordersQuery = _context.Orders
                .Where(o => o.UserId == userId) 
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product).AsQueryable(); 
                                                                  

                
            if (FilterStatus.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.Status == FilterStatus.Value);
            }
            ordersQuery = ordersQuery.OrderByDescending(o => o.CreatedAt);

            var orders = await ordersQuery.ToListAsync(); // Thực thi truy vấn

            var viewModel = new OrderHistoryViewModel
            {
                Orders = orders.Select(o => new OrderSummaryDto
                {
                    OrderId = o.Id,
                    CreatedAt = o.CreatedAt, 
                    Status = o.Status, 
                    Total = o.Total, 
                    ShippingFee = o.ShippingFee, 
                    Discount = o.Discount, 
                    Items = o.Items.Select(i => new OrderItemDto
                    {
                        ProductId =i.ProductId,
                        Quantity = i.Quantity, 
                        UnitPrice = i.UnitPrice, 
                        ProductName = i.Product.Name 
                    }).ToList()
                }).ToList(),

                FilterStatus = FilterStatus,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (order == null || order.UserId != userId)
            {
                return NotFound();
            }
            if (order.Status == OrderStatus.Pending) 
            {
                order.Status = OrderStatus.Cancelled; 
                await _context.SaveChangesAsync();
            }       

            return RedirectToAction("Index");
        }
    }
}
