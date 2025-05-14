using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sweets.Models;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sweets.Models;

namespace Sweets.Controllers
{
    public class ProductManufacturingsController : BaseController
    {
        private readonly SweetContext _context;

        public ProductManufacturingsController(SweetContext context) : base(context)
        {
            _context = context;
        }

        // GET: ProductManufacturings
        public async Task<IActionResult> Index()
        {
            var sweetContext = _context.ProductManufacturings
                .Include(e => e.Employee)
                .Include(p => p.FinishedProduct);
            return View(await sweetContext.ToListAsync());
        }

        // GET: ProductManufacturings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var productManufacturing = await _context.ProductManufacturings
                .Include(p => p.Employee)
                .Include(p => p.FinishedProduct)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (productManufacturing == null) return NotFound();

            return View(productManufacturing);
        }

        // GET: ProductManufacturings/Create
        public IActionResult Create()
        {
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "Id", "FullName");
            ViewData["FinishedProductId"] = new SelectList(_context.FinishedProducts, "Id", "Name");
            return View();
        }

        // POST: ProductManufacturings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FinishedProductId,Quantity,ManufactureDate,EmployeeID")] ProductManufacturing productManufacturing)
        {
            
            productManufacturing.ManufactureDate = DateOnly.FromDateTime(DateTime.Now);

           
            var product = await _context.FinishedProducts
                                         .FirstOrDefaultAsync(p => p.Id == productManufacturing.FinishedProductId);

            if (product == null)
            {
                return NotFound();
            }

           
            var ingredients = await _context.Ingredients
                                            .Where(i => i.ProductID == productManufacturing.FinishedProductId)
                                            .Include(i => i.RawMaterial)
                                            .ToListAsync();

            if (ingredients == null || ingredients.Count == 0)
            {
                return NotFound("Не найдено ингредиентов для этого продукта.");
            }

            decimal totalCost = 0;
            foreach (var ingredient in ingredients)
            {
                var rawMaterial = ingredient.RawMaterial;

                if (rawMaterial == null)
                {
                    continue;
                }
                if(rawMaterial.Quantity<(double)ingredient.Quantity * productManufacturing.Quantity)
                {
                    ModelState.AddModelError(string.Empty, $"Недостаточно сырья: {rawMaterial.Name}, для производства {productManufacturing.Quantity} единиц продукции.");
                    ViewBag.FinishedProductId = new SelectList(_context.FinishedProducts, "Id", "Name", productManufacturing.FinishedProductId);
                    ViewBag.EmployeeID = new SelectList(_context.Employees, "Id", "FullName", productManufacturing.EmployeeID);
                    return View(productManufacturing);
                }
                decimal costPerUnit = (decimal)rawMaterial.TotalCost / (decimal)rawMaterial.Quantity;
                decimal ingredientCost = costPerUnit * (decimal)ingredient.Quantity;
                totalCost += ingredientCost * (decimal)productManufacturing.Quantity;
                rawMaterial.Quantity -= (double)ingredient.Quantity * (double)productManufacturing.Quantity;
                rawMaterial.TotalCost -= (double)ingredientCost * productManufacturing.Quantity;
                _context.Update(rawMaterial);

            }
            product.Quantity += productManufacturing.Quantity;
            product.TotalCost += (double)totalCost;
            _context.Add(productManufacturing);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: ProductManufacturings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var productManufacturing = await _context.ProductManufacturings.FindAsync(id);
            if (productManufacturing == null) return NotFound();

            ViewData["EmployeeID"] = new SelectList(_context.Employees, "Id", "Name", productManufacturing.EmployeeID);
            ViewData["FinishedProductId"] = new SelectList(_context.FinishedProducts, "Id", "Name", productManufacturing.FinishedProductId);
            return View(productManufacturing);
        }

        // POST: ProductManufacturings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FinishedProductId,Quantity,ManufactureDate,EmployeeID")] ProductManufacturing productManufacturing)
        {
            if (id != productManufacturing.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productManufacturing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductManufacturingExists(productManufacturing.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["EmployeeID"] = new SelectList(_context.Employees, "Id", "Name", productManufacturing.EmployeeID);
            ViewData["FinishedProductId"] = new SelectList(_context.FinishedProducts, "Id", "Name", productManufacturing.FinishedProductId);
            return View(productManufacturing);
        }

        // GET: ProductManufacturings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var productManufacturing = await _context.ProductManufacturings
                .Include(p => p.Employee)
                .Include(p => p.FinishedProduct)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (productManufacturing == null) return NotFound();

            return View(productManufacturing);
        }

        // POST: ProductManufacturings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productManufacturing = await _context.ProductManufacturings.FindAsync(id);
            if (productManufacturing != null)
            {
                _context.ProductManufacturings.Remove(productManufacturing);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductManufacturingExists(int id)
        {
            return _context.ProductManufacturings.Any(e => e.Id == id);
        }
    }
}
