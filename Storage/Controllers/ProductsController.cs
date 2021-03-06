﻿using System;
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

        // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/

        // Updated to use async and do Select against the StorageContext
        public async Task<IActionResult> ProductView()
        {
            IEnumerable<ProductViewModel> productsViewList = await db.Product
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Count = p.Count,
                    InventoryValue = p.Price * p.Count
                }).ToListAsync();

            return View(productsViewList); // It's possible to specify which index the model should go to 
        }

        // GET: Products

        // Getting list of Products

        //[Route("")]
        //[Route("/Home") ]
        //[Route("/Home/Index")]
        public async Task<IActionResult> Index(string category = null)  // category is bound in index.cshtml (name="Category")
        {
            // https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/search?view=aspnetcore-3.1
            // Filtering is done in database. 

            // One way to do it:
            //var products = 
            //     db.Product.Where(p => string.IsNullOrEmpty(category) || p.Category.Contains(category));
            // For me to remember how the code above works: 
            // for conditional logical operators:If IsNullOrEmpty test is true, second test is not evaluated

            // Another way, maybe looks better:
            var products = string.IsNullOrEmpty(category) ? db.Product : db.Product.Where(p => p.Category.Contains(category));

            // Contains() works with upper/lower case and leading/trailing whitspace etc
            return View(await products.ToListAsync());

            // Ingen skillnad på SQL om man har "await ... ToListAsync()"  i return eller där products listan populeras
            // (vad jag kunde se i SQL loggningen). Men kanske snyggare att göra await sist.
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
