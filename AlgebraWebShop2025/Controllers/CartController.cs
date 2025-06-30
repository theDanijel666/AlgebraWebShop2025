using AlgebraWebShop2025.Data;
using AlgebraWebShop2025.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace AlgebraWebShop2025.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        public const string SessionKeyName = "_cart";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string message)
        {
            List<CartItem> cart=HttpContext.Session.
                GetObjectFromJson<List<CartItem>>(SessionKeyName) ?? new List<CartItem>();

            decimal sum = 0;
            foreach (CartItem item in cart) 
            {
                sum += item.getTotal();
                item.Product.Images=_context.Image.Where(i=>i.ProductId==item.Product.Id).ToList();
            }
            ViewBag.TotalPrice = sum;

            if(!String.IsNullOrEmpty(message)) ViewBag.Cart=message;

            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, decimal quantity)
        {
            List<CartItem> session_items = HttpContext.Session.
                GetObjectFromJson<List<CartItem>>(SessionKeyName) ?? new List<CartItem>();

            if (session_items.Count == 0)
            {
                CartItem cartItem = new CartItem()
                {
                    Product = _context.Product.Find(productId),
                    Quantity = quantity
                };
                session_items.Add(cartItem);

                HttpContext.Session.SetObjectAsJson(SessionKeyName, session_items);
            }
            else
            {
                int product_index = IsExistingInCart(productId);
                if (product_index == -1)
                {
                    CartItem cartItem = new CartItem()
                    {
                        Product = _context.Product.Find(productId),
                        Quantity = quantity
                    };
                    session_items.Add(cartItem);
                }
                else
                {
                    session_items[product_index].Quantity += quantity;
                }

                HttpContext.Session.SetObjectAsJson(SessionKeyName, session_items);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int productId)
        {
            List<CartItem> cart = HttpContext.Session.
                GetObjectFromJson<List<CartItem>>(SessionKeyName);
            int product_index=IsExistingInCart(productId);

            if(product_index>=0) cart.RemoveAt(product_index);

            HttpContext.Session.SetObjectAsJson(SessionKeyName, cart);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, decimal quantity)
        {
            List<CartItem> cart = HttpContext.Session.
                GetObjectFromJson<List<CartItem>>(SessionKeyName);
            int product_index = IsExistingInCart(productId);

            string msg="";
            if(quantity<0)
            {
                product_index = -1;
                msg = "Quantity can't be negative!";
            }

            if (product_index >= 0)
            {
                var prod=_context.Product.Find(productId);
                if (prod.Quantity < quantity)
                {
                    quantity= prod.Quantity;
                    msg = "Quantity set to available Quantity!";
                }
                cart[product_index].Quantity = quantity;
                if(quantity==0)
                {
                    cart.RemoveAt(product_index);
                    msg = "Product removed from cart.";
                }
                HttpContext.Session.SetObjectAsJson(SessionKeyName, cart);
            }

            return RedirectToAction(nameof(Index), new { message = msg });
        }

        private int IsExistingInCart(int productId) 
        {
            List<CartItem> cart=HttpContext.Session.
                GetObjectFromJson<List<CartItem>>(SessionKeyName);
            for(int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.Id== productId)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
