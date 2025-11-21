namespace CuaHangNhacCu.ViewModels
{

    public class BestSellerDto
    {
        public string ProductName { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal Price { get; set; } 
    }
    public class DashboardViewModel
    {
        public int TotalCustomers { get; set; }


        public int TotalOrdersInWeek { get; set; }


        public decimal TotalProfitInMonth { get; set; }

        public List<BestSellerDto> BestSellingProducts { get; set; }

    
        public Dictionary<string, int> ProductCountByCategory { get; set; }
    }
}
