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
    public class IngredientsController : BaseController
    {
        private readonly SweetContext _context;

        public IngredientsController(SweetContext context) : base(context)
        {
            
                _context = context;
            
        }

        // GET: Ingredients
        public async Task<IActionResult> Index()
        {
            var ingredients = await _context.Ingredients
                .Include(i => i.Product)
                .Include(i => i.RawMaterial)
                .ToListAsync();

            ViewBag.Products = await _context.FinishedProducts.ToListAsync();
            if (ingredients == null)
            {
                return View(new List<Ingredient>());
            }

            return View(ingredients);
        }



        // GET: Ingredients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients
                .Include(i => i.Product)
                .Include(i => i.RawMaterial)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        
        // GET: Ingredients/Create
        public IActionResult Create(int? productId)
        {
            ViewBag.ProductID = productId;
            ViewBag.RawMaterialID = new SelectList(_context.RawMaterials, "Id", "Name");
            ViewBag.Products = new SelectList(_context.FinishedProducts, "Id", "Name");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductID,RawMaterialID,Quantity")] Ingredient ingredient)
        {
            if (_context.Ingredients.Any(i => i.ProductID == ingredient.ProductID && i.RawMaterialID == ingredient.RawMaterialID))
            {
                ModelState.AddModelError("", "Такой ингредиент уже существует!");
            ViewBag.Products = new SelectList(_context.FinishedProducts, "Id", "Name", ingredient.ProductID); 
            ViewBag.RawMaterialID = new SelectList(_context.RawMaterials, "Id", "Name", ingredient.RawMaterialID); 
            }
            else { 
                _context.Add(ingredient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

          
            return View(ingredient);
        }



        // GET: Ingredients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }
            ViewData["ProductID"] = new SelectList(_context.FinishedProducts, "Id", "Name", ingredient.ProductID);
            ViewData["RawMaterialID"] = new SelectList(_context.RawMaterials, "Id", "Name", ingredient.RawMaterialID);
            return View(ingredient);
        }

        // POST: Ingredients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductID,RawMaterialID,Quantity")] Ingredient ingredient)
        {
            if (id != ingredient.Id)
            {
                return NotFound();
            }

            if (_context.Ingredients.Any(i => i.ProductID == ingredient.ProductID && i.RawMaterialID == ingredient.RawMaterialID && i.Id != ingredient.Id))
            {
                ViewBag.ErrorMessage = "Такой ингредиент уже существует!";

                ViewBag.ProductID = new SelectList(_context.FinishedProducts, "Id", "Name", ingredient.ProductID);
                ViewBag.RawMaterialID = new SelectList(_context.RawMaterials, "Id", "Name", ingredient.RawMaterialID);

                return View(ingredient);
            }

            try
            {
                _context.Update(ingredient);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IngredientExists(ingredient.Id))
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


        // GET: Ingredients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients
                .Include(i => i.Product)
                .Include(i => i.RawMaterial)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        // POST: Ingredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient != null)
            {
                _context.Ingredients.Remove(ingredient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IngredientExists(int id)   
        {
            return _context.Ingredients.Any(e => e.Id == id);
        }
    }
}
