using CuaHangNhacCu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CuaHangNhacCu.Data.Seeder
{
    public class TestOrderSeeder
    {
        public static async Task SeedOrdersAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            if (await context.Orders.AnyAsync())
            {
                Console.WriteLine("Orders already exist. Skipping test seed.");
                return;
            }

            var userManager = services.GetRequiredService<UserManager<User>>();

            var user = await userManager.FindByEmailAsync("customer1@example.com"); //PARWORD: Customer@123
            if (user == null)
            {
                Console.WriteLine("Test user 'customer1@example.com' not found. Run 'seed-data' first.");
                return;
            }

            var product1 = await context.Products.FirstOrDefaultAsync(p => p.Name == "Yamaha Grand Piano");
            var product2 = await context.Products.FirstOrDefaultAsync(p => p.Name == "Fender Stratocaster");
            if (product1 == null || product2 == null)
            {
                Console.WriteLine("Test products not found. Run 'seed-data' first.");
                return;
            }

            var order1 = new Order
            {
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                Status = OrderStatus.Pending,
                Items = new List<OrderItem> { new OrderItem { ProductId = product1.Id, Quantity = 1, UnitPrice = product1.Price } }
            };
            var order2 = new Order
            {
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                Status = OrderStatus.Processing,
                Items = new List<OrderItem> { new OrderItem { ProductId = product2.Id, Quantity = 2, UnitPrice = product2.Price } }
            };
            var order3 = new Order
            {
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Shipped,
                Items = new List<OrderItem> { new OrderItem { ProductId = product1.Id, Quantity = 1, UnitPrice = product1.Price } }
            };

            await context.Orders.AddRangeAsync(order1, order2, order3);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Test orders seeded.");
        }

        public static async Task ClearOrdersAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.ExecuteSqlRawAsync("DELETE FROM OrderItems");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Orders");
            Console.WriteLine("✅ All test orders cleared.");
        }
    }
}
