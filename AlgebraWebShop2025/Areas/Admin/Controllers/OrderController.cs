using AlgebraWebShop2025.Data;
using AlgebraWebShop2025.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlgebraWebShop2025.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/Order
        public async Task<IActionResult> Index()
        {
            return View(await _context.Order.ToListAsync());
        }

        // GET: Admin/Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            order.OrderItems=_context.OrderItem.Where(oi=>oi.OrderId== id).ToList();
            foreach (var item in order.OrderItems) 
                item.ProductTitle = _context.Product.Find(item.ProductId).Title;

            return View(order);
        }

        // GET: Admin/Order/Create
        public IActionResult Create()
        {
            Order order = new Order();
            order.DateCreated = DateTime.Now;
            order.Total = 0;

            ViewBag.Users = new SelectList(_userManager.Users, "Id", "Email");

            return View(order);
        }

        // POST: Admin/Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DateCreated,Total,BillingFirstName,BillingLastName,BillingEmail,BillingPhone,BillingAddress,BillingCity,BillingZIP,BillingCountry,ShippingFirstName,ShippingLastName,ShippingEmail,ShippingPhone,ShippingAddress,ShippingCity,ShippingZIP,ShippingCountry,Message,UserId")] 
        Order order, string ShippingSameAsBilling)
        {
            ModelState.Remove("ShippingSameAsBilling");
            ModelState.Remove("OrderItems");

            if (order.Message.IsNullOrEmpty()) order.Message = "";
            ModelState.Remove("Message");

            if (ShippingSameAsBilling == "on")
            {
                order.ShippingFirstName = order.BillingFirstName;
                ModelState.Remove("ShippingFirstName");
                order.ShippingLastName = order.BillingLastName;
                ModelState.Remove("ShippingLastName");
                order.ShippingEmail = order.BillingEmail;
                ModelState.Remove("ShippingEmail");
                order.ShippingPhone = order.BillingPhone;
                ModelState.Remove("ShippingPhone");
                order.ShippingAddress = order.BillingAddress;
                ModelState.Remove("ShippingAddress");
                order.ShippingCity = order.BillingCity;
                ModelState.Remove("ShippingCity");
                order.ShippingCountry = order.BillingCountry;
                ModelState.Remove("ShippingCountry");
                order.ShippingZIP = order.BillingZIP;
                ModelState.Remove("ShippingZIP");
            }

            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index","OrderItem",new { orderid=order.Id });
            }

            ViewBag.Users = new SelectList(_userManager.Users, "Id", "Email");
            return View(order);
        }

        // GET: Admin/Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.OrderItems = _context.OrderItem.Where(oi => oi.OrderId == id).ToList();
            foreach (var item in order.OrderItems)
                item.ProductTitle = _context.Product.Find(item.ProductId).Title;

            ViewBag.Users = new SelectList(_userManager.Users,"Id","Email");

            return View(order);
        }

        // POST: Admin/Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DateCreated,Total,BillingFirstName,BillingLastName,BillingEmail,BillingPhone,BillingAddress,BillingCity,BillingZIP,BillingCountry,ShippingFirstName,ShippingLastName,ShippingEmail,ShippingPhone,ShippingAddress,ShippingCity,ShippingZIP,ShippingCountry,Message,UserId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (String.IsNullOrEmpty(order.Message))
            {
                ModelState.Remove("Message");
                order.Message= string.Empty;
            }
            
            
            ModelState.Remove("OrderItems");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            order.OrderItems = _context.OrderItem.Where(oi => oi.OrderId == id).ToList();
            foreach (var item in order.OrderItems)
                item.ProductTitle = _context.Product.Find(item.ProductId).Title;

            ViewBag.Users = new SelectList(_userManager.Users, "Id", "Email");
            return View(order);
        }

        // GET: Admin/Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                var orderitems=_context.OrderItem.Where(oi=>oi.OrderId== order.Id).ToList();
                foreach (var item in orderitems)
                {
                    var prod = _context.Product.Find(item.ProductId);
                    prod.Quantity += item.Quantity;
                    _context.Update(prod);
                }
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}
