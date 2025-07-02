using AlgebraWebShop2025.Data;
using AlgebraWebShop2025.Extensions;
using AlgebraWebShop2025.Models;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AlgebraWebShop2025.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public const string SessionKeyName = "_cart";

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

        public IActionResult SingleProduct(int id)
        {
            var product = _context.Product.Find(id);
            if (product == null) return RedirectToAction(nameof(Product));

            product.Images=_context.Image.Where(i=>i.ProductId==id).ToList();
            product.ProductCategories=_context.ProductCategory.Where(p=>p.ProductId==id).ToList();
            foreach(var item in product.ProductCategories)
            {
                item.CategoryTitle = _context.Category.Find(item.CategoryId).Title;
            }

            return View(product);
        }

        public IActionResult Order(List<string> errors)
        {
            if (errors == null) errors= new List<string>();
            string msg = CheckCart();
            if (msg!="OK")
            {
                msg = "Cart: " + msg;
                errors.Add(msg);
            }
            List<CartItem> cart=HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName) 
                ?? new List<CartItem>();
            if(cart.Count==0)
            {
                ViewBag.OrderButton = "disabled=\"disabled\"";
            }
            decimal total = 0;
            foreach(var item in cart)
            {
                item.Product.Images=_context.Image.Where(i=>i.ProductId==item.Product.Id).ToList();
                total += item.getTotal();
            }

            ViewBag.TotalPrice=total;

            ViewBag.Errors = errors;

            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOrder([Bind("Total," +
            "BillingFirstName,BillingLastName,BillingEmail,BillingPhone,BillingAddress,BillingCity,BillingZIP,BillingCountry," +
            "ShippingFirstName,ShippingLastName,ShippingEmail,ShippingPhone,ShippingAddress,ShippingCity,ShippingZIP,ShippingCountry," +
            "Message")] Order order,bool ShippingSameAsBilling)
        {
            var modelErrors=new List<string>();

            string msg = CheckCart();
            if (msg != "OK")
            {
                msg = "Cart: " + msg;
                modelErrors.Add(msg);
            }
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName) 
                ?? new List<CartItem>();
            if (cart.Count == 0)
            {
                return RedirectToAction(nameof(Order), new { errors = modelErrors });
            }

            //TODO: sync shipping and billing if checked when they are the same!

            //TODO: remove useless validations and bind data
            

            if (ModelState.IsValid)
            {
                //TODO: if no model errors make order!
            }
            else
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        modelErrors.Add(modelError.ErrorMessage);
                    }
                }
            }


            return RedirectToAction(nameof(Order), new { errors = modelErrors });
        }

        private string CheckCart()
        {
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName)
                ?? new List<CartItem>();
            if (cart.Count == 0) return "Cart is empty";

            string message = "";
            for(int i = 0; i < cart.Count; i++)
            {
                var prod = _context.Product.Find(cart[i].Product.Id);
                if (prod.Quantity < cart[i].Quantity)
                {
                    cart[i].Quantity = prod.Quantity;
                    message += " " + prod.Title + " quantity set to available quantity.";
                }

                if (cart[i].Quantity == 0)
                {
                    message += " The item " + cart[i].Product.Title + " had to be remove because no stock!";
                    cart.RemoveAt(i);
                    i--;
                }
            }

            if (message.Length > 0)
            {
                HttpContext.Session.SetObjectAsJson(SessionKeyName, cart);
                return message;
            }

            return "OK";
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
