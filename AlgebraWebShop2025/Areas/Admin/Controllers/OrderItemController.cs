using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlgebraWebShop2025.Data;
using AlgebraWebShop2025.Models;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreGeneratedDocument;

namespace AlgebraWebShop2025.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderItemController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/OrderItem
        public async Task<IActionResult> Index(int orderid)
        {
            if (_context.Order.Where(o=>o.Id==orderid).FirstOrDefault()==null) 
                return RedirectToAction("Index", "Order");
            var items = await _context.OrderItem.Where(oi=>oi.OrderId==orderid).ToListAsync();
            decimal total = 0;
            foreach (var item in items)
            {
                item.ProductTitle = _context.Product.Find(item.ProductId).Title;
                total += (item.Quantity * item.Price * (1 - item.Discount / 100));
            }
            ViewBag.Total = total;
            ViewBag.OrderId = orderid;
            return View(items);
        }

        // GET: Admin/OrderItem/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            orderItem.ProductTitle = _context.Product.Find(orderItem.ProductId).Title;

            return View(orderItem);
        }

        // GET: Admin/OrderItem/Create
        public IActionResult Create(int orderid)
        {
            if (_context.Order.Where(o => o.Id == orderid).FirstOrDefault() == null)
                return RedirectToAction("Index", "Order");

            OrderItem item = new OrderItem();
            item.OrderId = orderid;
            item.Price = 0;
            item.Discount = 0;
            item.Quantity = 1;

            var products = new SelectList(_context.Product, "Id", "Title");
            ViewBag.Products = products;
            return View(item);
        }

        // POST: Admin/OrderItem/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderId,ProductId,Quantity,Price,Discount")] OrderItem orderItem)
        {
            //ModelState.Remove("Id");
            ModelState.Remove("ProductTitle");
            if (ModelState.IsValid)
            {
                _context.Add(orderItem);
                var product = _context.Product.Find(orderItem.ProductId);
                product.Quantity-=orderItem.Quantity;
                _context.Update(product);
                await _context.SaveChangesAsync();
                UpdateOrderTotal(orderItem.OrderId);
                return RedirectToAction(nameof(Edit), new { id = orderItem.Id, isNew = true });
            }
            return View(orderItem);
        }

        // GET: Admin/OrderItem/Edit/5
        public async Task<IActionResult> Edit(int? id,bool isNew)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItem.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(orderItem.ProductId);

            if (isNew)
            {
                orderItem.Price = product.Price;
                orderItem.Discount = product.Discount;
                _context.Update(orderItem);
                await _context.SaveChangesAsync();
                UpdateOrderTotal(orderItem.OrderId);
            }

            orderItem.ProductTitle = product.Title;

            ViewBag.QuantityMessage = "Available quantity: " + product.Quantity;
            
            return View(orderItem);
        }

        // POST: Admin/OrderItem/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderId,ProductId,Quantity,Price,Discount")] OrderItem orderItem)
        {
            if (id != orderItem.Id)
            {
                return NotFound();
            }

            var product = _context.Product.Find(orderItem.ProductId);

            ModelState.Remove("ProductTitle");
            if (ModelState.IsValid)
            {
                try
                {
                    var oldOrderItem = _context.OrderItem.Find(id);
                    var oldquantity = oldOrderItem.Quantity;

                    oldOrderItem.Price = orderItem.Price;
                    oldOrderItem.Discount = orderItem.Discount;
                    oldOrderItem.Quantity = orderItem.Quantity;

                    _context.Update(oldOrderItem);

                    product.Quantity -= (orderItem.Quantity - oldquantity);
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    UpdateOrderTotal(oldOrderItem.OrderId);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderItemExists(orderItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { orderid = orderItem.OrderId });
            }

            orderItem.ProductTitle = product.Title;
            ViewBag.QuantityMessage = "Available quantity: " + product.Quantity;
            return View(orderItem);
        }

        // GET: Admin/OrderItem/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            orderItem.ProductTitle = _context.Product.Find(orderItem.ProductId).Title;

            return View(orderItem);
        }

        // POST: Admin/OrderItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderItem = await _context.OrderItem.FindAsync(id);
            if (orderItem != null)
            {
                _context.OrderItem.Remove(orderItem);
                var product = await _context.Product.FindAsync(orderItem.ProductId);
                product.Quantity += orderItem.Quantity;
                _context.Update(product);
            }
            int orderid = orderItem.OrderId;
            await _context.SaveChangesAsync();
            UpdateOrderTotal(orderid);
            return RedirectToAction(nameof(Index), new { orderid = orderid });
        }

        private bool OrderItemExists(int id)
        {
            return _context.OrderItem.Any(e => e.Id == id);
        }

        private void UpdateOrderTotal(int orderId)
        {
            var order = _context.Order.Find(orderId);
            if (order == null) return;
            var orderItems = _context.OrderItem.Where(o => o.OrderId == orderId).ToList();
            decimal total = 0;
            foreach (var item in orderItems)
                total += (item.Price * item.Quantity * (1 - item.Discount / 100));
            order.Total = total;
            _context.Update(order);
            _context.SaveChanges();
        }
    }
}
