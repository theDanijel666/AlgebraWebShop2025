using System.Diagnostics;
using AlgebraWebShop2025.Data;
using AlgebraWebShop2025.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AlgebraWebShop2025.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Product(int? categoryId,decimal? priceFrom,decimal? priceTo,string? sort,int? per_page,int? page)
        {
            List<Product> products = _context.Product.
                Include(p=>p.Images).Include(p=>p.ProductCategories).ToList();

            if (categoryId != null)
            {
                products = products.Where(p => p.ProductCategories.Any(
                                        c => c.CategoryId == categoryId)).ToList();
            }

            if (priceFrom != null) { 
                products=products.Where(p=>p.Price>=priceFrom).ToList();
            }
            if (priceTo != null){
                products = products.Where(p => p.Price <= priceTo).ToList();
            }

            if (sort != null) 
            { 
                if(sort=="Price High to Low") products=products.OrderByDescending(p=>p.Price).ToList();
                if(sort=="Price Low to High") products = products.OrderBy(p => p.Price).ToList();
                if(sort=="Name A to Z") products = products.OrderBy(p => p.Title).ToList();
                if (sort == "Name Z to A") products = products.OrderByDescending(p => p.Title).ToList();
            }

            if (per_page == null) per_page = 20;
            if (page == null) page = 1;

            ViewBag.NumberOfPages=(int)Math.Ceiling((decimal)(products.Count)/(int)per_page);

            products=products.Skip((int)((page-1)*per_page)).Take((int)per_page).ToList();

            ViewBag.Categories=_context.Category.ToList();

            return View(products);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
