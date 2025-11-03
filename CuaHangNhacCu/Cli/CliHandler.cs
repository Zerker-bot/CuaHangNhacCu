using CuaHangNhacCu.Data.Seeder;

namespace CuaHangNhacCu.Cli;

public class CliHandler
{
    public static async Task Handle(string[] args, WebApplication app)
    {
        switch (args[0].ToLower())
        {
            case "seed-admin":
                await SeedAdmin(app);
                break;
            case "seed-data":
                await SeedData(app);
                break;
            default:
                Console.WriteLine("Unknown Command");
                break;
        }
    }

    public static async Task SeedAdmin(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
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


    public static async Task SeedData(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
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
