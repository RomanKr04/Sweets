using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sweets.Models;

namespace Sweets.Controllers
{
    public class FinishedProductsController : BaseController
    {
        private readonly SweetContext _context;

        public FinishedProductsController(SweetContext context) : base(context) 
        {
            _context = context;
        }

        // GET: FinishedProducts
        public async Task<IActionResult> Index()
        {
            var sweetContext = _context.FinishedProducts.Include(f => f.Unit);
            return View(await sweetContext.ToListAsync());
        }

        // GET: FinishedProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var finishedProduct = await _context.FinishedProducts
                .Include(f => f.Unit)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (finishedProduct == null)
            {
                return NotFound();
            }

            return View(finishedProduct);
        }

        // GET: FinishedProducts/Create
        public IActionResult Create()
        {
            var units = _context.Units.ToList();
            ViewBag.UnitID = new SelectList(units, "Id", "Name"); 
            return View();
        }



        // POST: FinishedProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,UnitID,Quantity,TotalCost")] FinishedProduct finishedProduct)
        {
                _context.Add(finishedProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
          
        }

        // GET: FinishedProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var finishedProduct = await _context.FinishedProducts.FindAsync(id);
            if (finishedProduct == null)
            {
                return NotFound();
            }
            ViewData["UnitID"] = new SelectList(_context.Units, "Id", "Name", finishedProduct.UnitID);
            return View(finishedProduct);
        }

        // POST: FinishedProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,UnitID,Quantity,TotalCost")] FinishedProduct finishedProduct)
        {
            if (id != finishedProduct.Id)
            {
                return NotFound();
            }

           
                try
                {
                    _context.Update(finishedProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FinishedProductExists(finishedProduct.Id))
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

        // GET: FinishedProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var finishedProduct = await _context.FinishedProducts
                .Include(f => f.Unit)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (finishedProduct == null)
            {
                return NotFound();
            }

            return View(finishedProduct);
        }

        // POST: FinishedProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var finishedProduct = await _context.FinishedProducts.FindAsync(id);
            if (finishedProduct != null)
            {
                _context.FinishedProducts.Remove(finishedProduct);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FinishedProductExists(int id)
        {
            return _context.FinishedProducts.Any(e => e.Id == id);
        }
    }
}
