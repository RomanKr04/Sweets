using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sweets.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace Sweets.Controllers
{
    public class ProductSalesController : BaseController
    {
        private readonly SweetContext _context;

        public ProductSalesController(SweetContext context) : base(context)
        {
            _context = context;
        }

        // GET: ProductSales
        public async Task<IActionResult> Index()
        {
            var sweetContext = _context.ProductSales
                .Include(p => p.Employee)
                .Include(p => p.Product);
            return View(await sweetContext.ToListAsync());
        }

        // GET: ProductSales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var productSale = await _context.ProductSales
                .Include(p => p.Employee)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (productSale == null) return NotFound();

            return View(productSale);
        }

        // GET: ProductSales/Create
        public IActionResult Create()
        {
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "Id", "FullName");
            ViewData["ProductID"] = new SelectList(_context.FinishedProducts, "Id", "Name");
            return View();
        }

        // POST: ProductSales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,Quantity,EmployeeID")] ProductSale productSale)
        {
            try
            {
                var sql = "SELECT sale_product(@p0, @p1::int, @p2)";
                await _context.Database.ExecuteSqlRawAsync(sql,
                    productSale.ProductID,
                    productSale.Quantity,
                    productSale.EmployeeID);

                return RedirectToAction(nameof(Index));
            }
            catch (PostgresException ex)
            {
                if (ex.Message.Contains("Недостаточно продуктов"))
                {
                    ModelState.AddModelError("Количество", ex.Message);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Ошибка: " + ex.Message);
                }
            }

            ViewData["EmployeeID"] = new SelectList(_context.Employees, "Id", "FullName", productSale.EmployeeID);
            ViewData["ProductID"] = new SelectList(_context.FinishedProducts, "Id", "Name", productSale.ProductID);
            return View(productSale);
        }


        // GET: ProductSales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var productSale = await _context.ProductSales.FindAsync(id);
            if (productSale == null) return NotFound();

            ViewData["EmployeeID"] = new SelectList(_context.Employees, "Id", "FullName", productSale.EmployeeID);
            ViewData["ProductID"] = new SelectList(_context.FinishedProducts, "Id", "Name", productSale.ProductID);
            return View(productSale);
        }

        // POST: ProductSales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductID,Quantity,TotalCost,SaleDate,EmployeeID")] ProductSale productSale)
        {
            if (id != productSale.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productSale);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductSaleExists(productSale.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["EmployeeID"] = new SelectList(_context.Employees, "Id", "FullName", productSale.EmployeeID);
            ViewData["ProductID"] = new SelectList(_context.FinishedProducts, "Id", "Name", productSale.ProductID);
            return View(productSale);
        }

        // GET: ProductSales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var productSale = await _context.ProductSales
                .Include(p => p.Employee)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (productSale == null) return NotFound();

            return View(productSale);
        }

        // POST: ProductSales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productSale = await _context.ProductSales.FindAsync(id);
            if (productSale != null)
            {
                _context.ProductSales.Remove(productSale);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductSaleExists(int id)
        {
            return _context.ProductSales.Any(e => e.Id == id);
        }
    }
}