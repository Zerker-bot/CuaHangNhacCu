using CuaHangNhacCu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CuaHangNhacCu.Data.Seeder
{
    public class TestDataSeeder
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

        public static async Task SeedCartAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            if (await context.Carts.AnyAsync())
            {
                Console.WriteLine("Carts already exist. Skipping test seed.");
                return;
            }

            var userManager = services.GetRequiredService<UserManager<User>>();

            var user = await userManager.FindByEmailAsync("customer1@example.com");
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

            var cart = new Cart
            {
                UserId = user.Id
            };
            await context.Carts.AddAsync(cart);
            await context.SaveChangesAsync();

            var cartItem1 = new CartItem
            {
                CartId = cart.Id,
                ProductId = product1.Id,
                Quantity = 1
            };
            var cartItem2 = new CartItem
            {
                CartId = cart.Id,
                ProductId = product2.Id,
                Quantity = 2
            };

            await context.CartItems.AddRangeAsync(cartItem1, cartItem2);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Test cart seeded for customer1@example.com.");
        }

        public static async Task ClearCartAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.ExecuteSqlRawAsync("DELETE FROM CartItems");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Carts");
            Console.WriteLine("✅ All test carts cleared.");
        }
    }
}
