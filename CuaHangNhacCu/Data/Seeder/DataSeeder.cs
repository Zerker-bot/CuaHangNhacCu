using CuaHangNhacCu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CuaHangNhacCu.Data.Seeder;

public static class DataSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        var customer = await userManager.GetUsersInRoleAsync("Customer");
        if (!customer.Any())
        {
            var customers = new List<User>
            {
                new() {
                    UserName = "customer1@example.com",
                    Email = "customer1@example.com",
                    FullName = "Nguyễn Văn A",
                    DateOfBirth = new DateTime(1995, 5, 10),
                    EmailConfirmed = true
                },
                new() {
                    UserName = "customer2@example.com",
                    Email = "customer2@example.com",
                    FullName = "Trần Thị B",
                    DateOfBirth = new DateTime(1998, 11, 25),
                    EmailConfirmed = true
                },
                new() {
                    UserName = "customer3@example.com",
                    Email = "customer3@example.com",
                    FullName = "Lê Văn C",
                    DateOfBirth = new DateTime(1990, 2, 18),
                    EmailConfirmed = true
                },
                new() {
                    UserName = "customer4@example.com",
                    Email = "customer4@example.com",
                    FullName = "Phạm Thị D",
                    DateOfBirth = new DateTime(2000, 8, 5),
                    EmailConfirmed = true
                },
                new() {
                    UserName = "customer5@example.com",
                    Email = "customer5@example.com",
                    FullName = "Hoàng Văn E",
                    DateOfBirth = new DateTime(1993, 3, 30),
                    EmailConfirmed = true
                }
            };

            foreach (var user in customers)
            {
                await userManager.CreateAsync(user, "Customer@123"); // password mặc định
                await userManager.AddToRoleAsync(user, "Customer");
            }
        }

        await context.SaveChangesAsync();

        if(!context.Brands.Any())
        {
            await context.Brands.AddRangeAsync(
                new Brand { Name = "Yamaha", Description = "High-quality instruments" },
                new Brand { Name = "Fender", Description = "Legendary guitars" },
                new Brand { Name = "Tanglewood", Description = "guitars" },
                new Brand { Name = "Casio", Description = "keyboard" }
            );
            await context.SaveChangesAsync();
        }

        if(!context.Categories.Any())
        {
            await context.Categories.AddRangeAsync(
                new Category { Name = "Piano", Description = "Piano" },
                new Category { Name = "Guitar", Description = "Guitar" },
                new Category { Name = "Drum", Description = "Drum" },
                new Category { Name = "Violin", Description = "Violin" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Products.Any())
        {
            var brands = context.Brands.ToList();
            var categories = context.Categories.ToList();

            var piano = new Product
            {
                Name = "Yamaha Grand Piano",
                Description = "Elegant and full-featured piano for professionals.",
                Price = 5000m,
                Cost = 3500m,
                Quantity = 5,
                CategoryId = categories.First(c => c.Name == "Piano").Id,
                BrandId = brands.First(b => b.Name == "Yamaha").Id,
                Images =
                [
                    new() { Url = "images/piano/a1.jpg", AltText = "Front view", IsPrimary = true },
                    new() { Url = "images/piano/a2.jpg", AltText = "Side view" },
                    new() { Url = "images/piano/a3.jpg", AltText = "Keyboard close-up" },
                    new() { Url = "images/piano/a4.jpg", AltText = "Pedals" }
                ]
            };

            var guitar = new Product
            {
                Name = "Fender Stratocaster",
                Description = "Iconic electric guitar with legendary tone.",
                Price = 1200m,
                Cost = 800m,
                Quantity = 10,
                CategoryId = categories.First(c => c.Name == "Guitar").Id,
                BrandId = brands.First(b => b.Name == "Fender").Id,
                Images =
                [
                    new() { Url = "images/guitar/b1.png", AltText = "Front view", IsPrimary = true },
                    new() { Url = "images/guitar/b2.png", AltText = "Back view" }
                ]
            };

            var drum = new Product
            {
                Name = "Yamaha Drum Set",
                Description = "Full acoustic drum set with premium build quality.",
                Price = 2000m,
                Cost = 1500m,
                Quantity = 7,
                CategoryId = categories.First(c => c.Name == "Drum").Id,
                BrandId = brands.First(b => b.Name == "Yamaha").Id,
                Images =
                [
                    new() { Url = "images/drum/c1.jpg", AltText = "Front view", IsPrimary = true },
                    new() { Url = "images/drum/c2.jpg", AltText = "Top view" },
                    new() { Url = "images/drum/c3.jpg", AltText = "Side view" }
                ]
            };

            var violin = new Product
            {
                Name = "Tanglewood Violin Classic",
                Description = "Beautiful wooden violin with rich tone.",
                Price = 900m,
                Cost = 600m,
                Quantity = 8,
                CategoryId = categories.First(c => c.Name == "Violin").Id,
                BrandId = brands.First(b => b.Name == "Tanglewood").Id,
                Images =
                [
                    new() { Url = "images/violin/d1.jpg", AltText = "Front view", IsPrimary = true },
                    new() { Url = "images/violin/d2.jpg", AltText = "Bow included" },
                    new() { Url = "images/violin/d3.jpg", AltText = "Close-up" }
                ]
            };

            await context.Products.AddRangeAsync(piano, guitar, drum, violin);
            await context.SaveChangesAsync();
        }

        if (!context.Reviews.Any())
        {
            var users = await userManager.GetUsersInRoleAsync("Customer");
            var products = context.Products.ToList();

            var reviews = new List<Review>
            {
                new() {
                    ProductId = products.First(p => p.Name == "Yamaha Grand Piano").Id,
                    UserId = users[0].Id,
                    Rating = 5,
                    Content = "Âm thanh tuyệt vời, cảm giác phím rất tốt!",
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    IsApproved = true
                },
                new()
                {
                    ProductId = products.First(p => p.Name == "Fender Stratocaster").Id,
                    UserId = users[1].Id,
                    Rating = 4,
                    Content = "Âm thanh guitar đỉnh cao, rất đáng tiền.",
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    IsApproved = true
                },
                new()
                {
                    ProductId = products.First(p => p.Name == "Yamaha Drum Set").Id,
                    UserId = users[2].Id,
                    Rating = 5,
                    Content = "Trống âm vang tốt, chất lượng build tuyệt vời.",
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    IsApproved = true
                },
                new()
                {
                    ProductId = products.First(p => p.Name == "Tanglewood Violin Classic").Id,
                    UserId = users[3].Id,
                    Rating = 4,
                    Content = "Violin âm thanh rất ấm, hợp người mới bắt đầu.",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    IsApproved = true
                }
            };

            await context.Reviews.AddRangeAsync(reviews);
            await context.SaveChangesAsync();
        }
    }
}
