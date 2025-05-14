using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sweets.Models;

namespace Sweets.Controllers
{
    public class RawMaterialPurchasesController : BaseController
    {
        private readonly SweetContext _context;

        public RawMaterialPurchasesController(SweetContext context) : base(context) 
        {
            _context = context;
           

        }

        // GET: RawMaterialPurchases
        public async Task<IActionResult> Index()
        {
            ViewBag.EmployeeID = new SelectList(_context.Employees, "Id", "Name");
            ViewBag.RawMaterialID = new SelectList(_context.RawMaterials, "Id", "FullName");
            var sweetContext = _context.RawMaterialPurchases.Include(r => r.Employee).Include(r => r.RawMaterial);
            return View(await sweetContext.ToListAsync());
        }

        // GET: RawMaterialPurchases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rawMaterialPurchase = await _context.RawMaterialPurchases
                .Include(r => r.Employee)
                .Include(r => r.RawMaterial)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rawMaterialPurchase == null)
            {
                return NotFound();
            }

            return View(rawMaterialPurchase);
        }

        // GET: RawMaterialPurchases/Create
        public IActionResult Create()
        {
            ViewBag.RawMaterialID = new SelectList(_context.RawMaterials.ToList(), "Id", "Name");
            ViewBag.EmployeeID = new SelectList(_context.Employees.ToList(), "Id", "FullName");

            return View();
        }


        // POST: RawMaterialPurchases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RawMaterialID,Quantity,TotalCost,PurchaseDate,EmployeeID")] RawMaterialPurchase rawMaterialPurchase)
        {
          var rawMaterial = await _context.RawMaterials.FindAsync(rawMaterialPurchase.RawMaterialID);
            if (rawMaterial == null)
            {
                return NotFound();
            }
            rawMaterial.Quantity += rawMaterialPurchase.Quantity;
            rawMaterial.TotalCost+= rawMaterialPurchase.TotalCost;
            var bydget = _context.Budgets.FirstOrDefault();
            if(bydget==null || bydget.TotalAmount < (decimal)rawMaterialPurchase.TotalCost)
            {
                ModelState.AddModelError("", "Недостаточно средств в бюджете.");
                ViewBag.RawMaterialID = new SelectList(_context.RawMaterials, "Id", "Name", rawMaterialPurchase.RawMaterialID);
                ViewBag.EmployeeID = new SelectList(_context.Employees, "Id", "FullName", rawMaterialPurchase.EmployeeID);
                return View(rawMaterialPurchase);
            }
            bydget.TotalAmount -= (decimal)rawMaterialPurchase.TotalCost;
            rawMaterialPurchase.PurchaseDate = DateOnly.FromDateTime(DateTime.Now);
            _context.Add(rawMaterialPurchase);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: RawMaterialPurchases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rawMaterialPurchase = await _context.RawMaterialPurchases.FindAsync(id);
            if (rawMaterialPurchase == null)
            {
                return NotFound();
            }
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "Id", "Id", rawMaterialPurchase.EmployeeID);
            ViewData["RawMaterialID"] = new SelectList(_context.RawMaterials, "Id", "Id", rawMaterialPurchase.RawMaterialID);
            return View(rawMaterialPurchase);
        }

        // POST: RawMaterialPurchases/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RawMaterialID,Quantity,TotalCost,PurchaseDate,EmployeeID")] RawMaterialPurchase rawMaterialPurchase)
        {
            if (id != rawMaterialPurchase.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rawMaterialPurchase);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RawMaterialPurchaseExists(rawMaterialPurchase.Id))
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
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "Id", "Id", rawMaterialPurchase.EmployeeID);
            ViewData["RawMaterialID"] = new SelectList(_context.RawMaterials, "Id", "Id", rawMaterialPurchase.RawMaterialID);
            return View(rawMaterialPurchase);
        }

        // GET: RawMaterialPurchases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rawMaterialPurchase = await _context.RawMaterialPurchases
                .Include(r => r.Employee)
                .Include(r => r.RawMaterial)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rawMaterialPurchase == null)
            {
                return NotFound();
            }

            return View(rawMaterialPurchase);
        }

        // POST: RawMaterialPurchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rawMaterialPurchase = await _context.RawMaterialPurchases.FindAsync(id);
            if (rawMaterialPurchase != null)
            {
                _context.RawMaterialPurchases.Remove(rawMaterialPurchase);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RawMaterialPurchaseExists(int id)
        {
            return _context.RawMaterialPurchases.Any(e => e.Id == id);
        }
      
   }
}
