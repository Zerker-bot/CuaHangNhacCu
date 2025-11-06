using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CuaHangNhacCu.Data; // Giả định DbContext của bạn
using CuaHangNhacCu.Models;
using CuaHangNhacCu.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

// Cần quyền Admin để truy cập dashboard
[Area("Admin")]

public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context; // DbContext
    private readonly UserManager<User> _userManager;

    public DashboardController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new DashboardViewModel();

        try
        {
            var customerUsers = await _userManager.GetUsersInRoleAsync("Customer");

            viewModel.TotalCustomers = customerUsers.Count;
        }
        catch (Exception ex)
        {
            viewModel.TotalCustomers = 0;
        }



        var deliveredOrders = _context.Orders
            .Where(o => o.Status == OrderStatus.Delivered)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product) 
            .ToList();
        //Tong so don trong tuan
        var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
        viewModel.TotalOrdersInWeek = deliveredOrders
      .Where(o => o.CreatedAt >= oneWeekAgo)
      .Count();

        // Loi nhan 1 thang
        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        viewModel.TotalProfitInMonth = deliveredOrders
         .Where(o => o.CreatedAt >= startOfMonth)
         .Sum(o => {
             decimal revenue = o.Total;
             decimal cogs = o.Items.Sum(item => item.Product.Cost * item.Quantity);
             return revenue - cogs;
         });
        // top 10
        viewModel.BestSellingProducts = await _context.OrderItems
            .GroupBy(oi => oi.ProductId)
            .Select(g => new BestSellerDto
            {
                TotalQuantitySold = g.Sum(oi => oi.Quantity),
                ProductName = g.First().Product.Name,
                Price = g.First().Product.Price
            })
            .OrderByDescending(dto => dto.TotalQuantitySold)
            .Take(10)
            .ToListAsync();
        //Co cau san pham
        viewModel.ProductCountByCategory = await _context.Products
            .GroupBy(p => p.Category.Name)
            .Select(g => new {
                CategoryName = g.Key,
                Count = g.Count()
            })
            .ToDictionaryAsync(x => x.CategoryName ?? "Không xác định", x => x.Count);

        return View(viewModel);
    }
}