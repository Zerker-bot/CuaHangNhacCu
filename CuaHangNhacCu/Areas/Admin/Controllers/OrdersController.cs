using CuaHangNhacCu.Areas.Admin.Models;
using CuaHangNhacCu.Data;
using CuaHangNhacCu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CuaHangNhacCu.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(OrderStatus status = OrderStatus.Pending)
        {
            var ordersQuery = _context.Orders
                                    .Where(o => o.Status == status)
                                    .Include(o => o.User)
                                    .Include(o => o.Items)
                                        .ThenInclude(oi => oi.Product)
                                    .OrderByDescending(o => o.CreatedAt);

            var viewModel = new OrderIndexViewModel
            {
                Orders = await ordersQuery.ToListAsync(),
                CurrentStatus = status      
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = newStatus;
            _context.Update(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã cập nhật trạng thái đơn hàng #{orderId} thành {newStatus.ToString()}.";

            return RedirectToAction(nameof(Index), new { status = order.Status });
        }
    }
}
