using CuaHangNhacCu.Data;
using CuaHangNhacCu.Models;
using CuaHangNhacCu.ViewModels.Product;
using CuaHangNhacCu.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CuaHangNhacCu.Controllers;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _ctx;

    public ProductController(ApplicationDbContext context)
    {
        _ctx = context;
    }

    public async Task<IActionResult> Index(
        string? sortOrder,
        int? maxPrice,
        string q,
        List<int> cat,
        List<int> brand,
        int page = 1,
        int pageSize = 10
    )
    {
        var totalItems = await _ctx.Products.CountAsync();

        IQueryable<Product> productsQuery = _ctx.Products;

        if(!q.IsNullOrEmpty()) 
        {
            productsQuery = productsQuery.Where(e => e.Name.Contains(q));
        }

        if(cat.Count > 0) 
        {
            productsQuery = productsQuery.Where(e => cat.Contains(e.CategoryId));
        }

        if(brand.Count > 0) 
        {
            productsQuery = productsQuery.Where(e => brand.Contains(e.BrandId));
        }

        if(!sortOrder.IsNullOrEmpty())
        {
            switch(sortOrder) {
                case "latest": 
                   productsQuery = productsQuery.OrderBy(e => e.CreatedAt);
                   break;
                case "price":
                   productsQuery = productsQuery.OrderBy(e => e.Price);
                   break;
                case "price_desc":
                   productsQuery = productsQuery.OrderByDescending(e => e.Price);
                   break;
            };
        }

        if(maxPrice is not null)
        {
               productsQuery = productsQuery.Where(e => e.Price < maxPrice);
        }

        List<Product> products = await productsQuery 
            .Include(e => e.Images)
            .Include(e => e.Brand)
            .Include(e => e.Category)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        var viewModel = new ProductIndex() 
        {
            Products = products,
            Categories = await _ctx.Categories.ToListAsync(),
            Brands = await _ctx.Brands.ToListAsync(),
            Pagination = new Pagination()
            {
                PageNumber = page,
                TotalPages = (int)Math.Ceiling((double) totalItems / pageSize)
            },
            Search = q,
            Cat = cat,
            Brand = brand,
            SortOrd = sortOrder,
            MaxPrice = maxPrice
        };
        return View(viewModel);
    }


    public async Task<IActionResult> Detail(int id)
    {
        return View();
    }

}
