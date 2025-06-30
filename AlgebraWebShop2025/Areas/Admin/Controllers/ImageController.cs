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

namespace AlgebraWebShop2025.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ImageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ImageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Image
        public async Task<IActionResult> Index(int productId)
        {
            if (productId == 0 || productId < 0)
                return RedirectToAction("Index", "Product");
            var product = _context.Product.Where(p => p.Id == productId).FirstOrDefault();
            if (product == null) return RedirectToAction("Index", "Product");
            
            List<Image> images = await _context.Image.
                Where(i=>i.ProductId==productId).ToListAsync();
            ViewBag.ProductId = productId;

            return View(images);
        }

        // GET: Admin/Image/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Image
                .FirstOrDefaultAsync(m => m.Id == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // GET: Admin/Image/Create
        public IActionResult Create(int productId)
        {
            if (productId == 0 || productId < 0)
                return RedirectToAction("Index", "Product");
            var product = _context.Product.Where(p => p.Id == productId).FirstOrDefault();
            if (product == null) return RedirectToAction("Index", "Product");

            var img=new Image { ProductId = productId };

            return View(img);
        }

        // POST: Admin/Image/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IsMain,Title,Description,URL,ProductId")] Image image, 
            IFormFile file)
        {
            if(file==null || file.Length == 0)
            {
                ModelState.AddModelError("ProductId", "You must upload a file!");
            }
            ModelState.Remove("URL");
            if (ModelState.IsValid)
            {
                string extension=System.IO.Path.GetExtension(file.FileName).ToLower();
                if(extension!=".jpg" && extension!=".jpeg" && extension != ".png")
                {
                    ModelState.AddModelError("ProductId", 
                        "File in incorrect format! Must be jpg or jpeg or png!");
                    return View(image);
                }

                string filename=image.ProductId.ToString()+DateTime.Now.Ticks.ToString()+file.FileName;

                string saveLocation = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot", "Media", "Images", filename);
                var fileStream=new FileStream(saveLocation,FileMode.Create);
                file.CopyTo(fileStream);
                fileStream.Flush();
                fileStream.Close();

                image.URL = "/Media/Images/" + filename;

                _context.Add(image);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index),new { productId = image.ProductId });
            }
            return View(image);
        }

        // GET: Admin/Image/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Image.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            return View(image);
        }

        // POST: Admin/Image/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IsMain,Title,Description,URL,ProductId")] Image image,
            IFormFile file)
        {
            if (id != image.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(file.Length > 0)
                    {
                        string extension = System.IO.Path.GetExtension(file.FileName).ToLower();
                        if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                        {
                            string filename = image.ProductId.ToString() + DateTime.Now.Ticks.ToString() + file.FileName;

                            string saveLocation = Path.Combine(Directory.GetCurrentDirectory(),
                                "wwwroot", "Media", "Images", filename);
                            var fileStream = new FileStream(saveLocation, FileMode.Create);
                            file.CopyTo(fileStream);
                            fileStream.Flush();
                            fileStream.Close();

                            string newURL= "/Media/Images/" + filename;

                            string oldfile = image.URL;
                            oldfile = oldfile.Remove(0, 1).Replace("/", "\\\\");
                            oldfile = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldfile);
                            if (System.IO.File.Exists(oldfile)) System.IO.File.Delete(oldfile);

                            image.URL = newURL;
                        }
                        else
                        {
                            ModelState.AddModelError("URL", "File format not supported!");
                            return View(image);
                        }
                    }
                    _context.Update(image);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImageExists(image.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index),new { productId = image.ProductId });
            }
            return View(image);
        }

        // GET: Admin/Image/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Image
                .FirstOrDefaultAsync(m => m.Id == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // POST: Admin/Image/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = await _context.Image.FindAsync(id);
            string file="";
            if (image != null)
            {
                file = image.URL;
                _context.Image.Remove(image);
            }

            await _context.SaveChangesAsync();

            
            file=file.Remove(0,1).Replace("/", "\\\\");
            file=System.IO.Path.Combine(Directory.GetCurrentDirectory(),"wwwroot" ,file);
            if(System.IO.File.Exists(file)) System.IO.File.Delete(file); 

            return RedirectToAction(nameof(Index));
        }

        private bool ImageExists(int id)
        {
            return _context.Image.Any(e => e.Id == id);
        }
    }
}
