using CuaHangNhacCu.Data.Seeder;

namespace CuaHangNhacCu.Cli;

public class CliHandler
{
    public static async Task Handle(string[] args, WebApplication app)
    {
        using var scope = app.Services.CreateScope(); //line 9, 10, Lấy services 1 lần, đỡ phải lặp lại using var trong các hàm
        var services = scope.ServiceProvider;         

        switch (args[0].ToLower())
        {
            case "seed-admin":
                await SeedAdmin(services);
                break;
            case "seed-data":
                await SeedData(services);
                break;
            case "seed-orders":
                try
                {
                    await TestOrderSeeder.SeedOrdersAsync(services);
                }
                catch (Exception ex) { Console.WriteLine("❌ Error: " + ex.Message); }
                break;
            case "clear-orders":
                try
                {
                    await TestOrderSeeder.ClearOrdersAsync(services);
                }
                catch (Exception ex) { Console.WriteLine("❌ Error: " + ex.Message); }
                break;
            default:
                Console.WriteLine("Unknown Command");
                break;
        }
    }

    public static async Task SeedAdmin(IServiceProvider services)
    {
        try
        {
            await SuperAdminSeeder.SeedSuperAdminAsync(services);
            Console.WriteLine("✅ Super Admin seeded.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Error seeding Super Admin: " + ex.Message);
        }
    }


    public static async Task SeedData(IServiceProvider services)
    {
        try
        {
            await DataSeeder.SeedDataAsync(services);
            Console.WriteLine("✅ Data seeded successful.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Error seeding data: " + ex.Message);
        }
    }
}
