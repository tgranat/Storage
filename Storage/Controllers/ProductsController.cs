using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Storage.Data;
using Storage.Models;

namespace Storage.Controllers
{
    public class ProductsController : Controller
    {
        private readonly StorageContext db;

        public ProductsController(StorageContext context)
        {
            db = context;
        }

        // IActionResult for the new product view model
        // First attempt (keeping this for reference)

        //public IActionResult ProductView()
        //{
        //    List<Product> dbProducts = db.Product.ToList();

        //    IEnumerable<ProductViewModel> productsViewList = dbProducts
        //        .Select(p => new ProductViewModel {
        //            Name = p.Name, 
        //            Price = p.Price, 
        //            Count = p.Count, 
        //            InventoryValue = p.Price * p.Count}).ToList();
        //    return View(productsViewList);
        //}

        // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/

        // Updated to use async and do Select against the StorageContext
        public async Task<IActionResult> ProductView()
        {
            IEnumerable<ProductViewModel> productsViewList = await db.Product
                .Select(p => new ProductViewModel
                {
                    Name = p.Name,
                    Price = p.Price,
                    Count = p.Count,
                    InventoryValue = p.Price * p.Count
                }).ToListAsync();

            return View(productsViewList);
        }

        // GET: Products

        // Getting list of Products

        public async Task<IActionResult> Index(string category)
        {
            // https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/search?view=aspnetcore-3.1
            // Filtering is done in database. I think? Depends on how it's implemented?
            IEnumerable<Product> products = 
                await db.Product.Where(p => string.IsNullOrEmpty(category) || p.Category.Contains(category)).ToListAsync();
            // For me to remember how the code above works: 
            // for conditional logical operators:If IsNullOrEmpty test is true, second test is not evaluated
            // Contains() works with upper/lower case and leading/trailing whitspace etc
            return View(products);
        }

        // GET: Products/Details/5

        // Getting a Product based on Id (from list of Products)

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await db.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create

        // Create new

        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create

        // Submitting form with new Product

        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Orderdate,Category,Shelf,Count,Description")] Product product)
        {
            if (ModelState.IsValid)
            {
                // TODO trim strings etc. before store
                db.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5

        // Choose Edit from list of Products
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await db.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5

        // Submitting updated Product

        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Orderdate,Category,Shelf,Count,Description")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // TODO trim strings etc. before store
                    db.Update(product);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Products/Delete/5

        // Get product to delete

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await db.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5

        // Submit delete

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await db.Product.FindAsync(id);
            db.Product.Remove(product);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return db.Product.Any(e => e.Id == id);
        }
    }
}
