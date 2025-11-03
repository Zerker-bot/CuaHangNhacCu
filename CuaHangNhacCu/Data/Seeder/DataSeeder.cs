using CuaHangNhacCu.Models;

namespace CuaHangNhacCu.Data.Seeder;

public class DataSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        if(!context.Brands.Any()) 
        {
            await context.Brands.AddRangeAsync(
                new Brand { Name = "Yamaha", Description = "High-quality instruments" },
                new Brand { Name = "Fender", Description = "Legendary guitars" },
                new Brand { Name = "Tanglewood", Description = "guitars" },
                new Brand { Name = "Casio", Description = "keyboard" }
            );
            await context.SaveChangesAsync();
        };

        if(!context.Categories.Any()) 
        {
            await context.Categories.AddRangeAsync(
                new Category { Name = "Piano", Description = "Piano" },
                new Category { Name = "Guitar", Description = "Guitar" },
                new Category { Name = "Drum", Description = "Drum" },
                new Category { Name = "Violin", Description = "Violin" }
            );
            await context.SaveChangesAsync();
        };

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
                Images = new List<ProductImage>
                {
                    new() { Url = "images/piano/a1.jpg", AltText = "Front view", IsPrimary = true },
                    new() { Url = "images/piano/a2.jpg", AltText = "Side view" },
                    new() { Url = "images/piano/a3.jpg", AltText = "Keyboard close-up" },
                    new() { Url = "images/piano/a4.jpg", AltText = "Pedals" }
                }
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
                Images = new List<ProductImage>
                {
                    new() { Url = "images/guitar/b1.png", AltText = "Front view", IsPrimary = true },
                    new() { Url = "images/guitar/b2.png", AltText = "Back view" }
                }
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
                Images = new List<ProductImage>
                {
                    new() { Url = "images/drum/c1.jpg", AltText = "Front view", IsPrimary = true },
                    new() { Url = "images/drum/c2.jpg", AltText = "Top view" },
                    new() { Url = "images/drum/c3.jpg", AltText = "Side view" }
                }
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
                Images = new List<ProductImage>
                {
                    new() { Url = "images/violin/d1.jpg", AltText = "Front view", IsPrimary = true },
                    new() { Url = "images/violin/d2.jpg", AltText = "Bow included" },
                    new() { Url = "images/violin/d3.jpg", AltText = "Close-up" }
                }
            };

            await context.Products.AddRangeAsync(piano, guitar, drum, violin);
            await context.SaveChangesAsync();
        }
    }
}
