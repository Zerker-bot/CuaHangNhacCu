using CuaHangNhacCu.ViewModels.Shared;

namespace CuaHangNhacCu.ViewModels.Product;

public class ProductIndex
{
    public List<Models.Product> Products { get; set; }
    public List<Models.Category> Categories { get; set; }
    public List<Models.Brand> Brands { get; set; }
    public Pagination Pagination { get; set; }
    public string? Search { get; set; }
    public string? SortOrd { get; set; }
    public List<int> Cat { get; set; }
    public List<int> Brand { get; set; }
    public int? MaxPrice { get; set; }
}
