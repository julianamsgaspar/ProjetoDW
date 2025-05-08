using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PawBuddy.Data;
using PawBuddy.Models;

namespace PawBuddy.Controllers
{
    [Authorize(Roles = "Admin")] // Só admin acessa 
    public class AdotamController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        //construtor
        public AdotamController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Adotam
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Adotam.Include(a => a.Animal).Include(a => a.Utilizador);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Adotam/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adotam = await _context.Adotam
                .Include(a => a.Animal)
                .Include(a => a.Utilizador)
                .FirstOrDefaultAsync(m => m.AnimalFK == id);
            if (adotam == null)
            {
                return NotFound();
            }

            return View(adotam);
        }

        // GET: Adotam/Create
        public IActionResult Create()
        {
            ViewData["AnimalFK"] = new SelectList(_context.Animal, "Id", "Cor");
            ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "Id", "CodPostal");
            return View();
        }

        // POST: Adotam/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,dateA,UtilizadorFK,AnimalFK")] Adotam adotam)
        {
            if (ModelState.IsValid)
            {
                _context.Add(adotam);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AnimalFK"] = new SelectList(_context.Animal, "Id", "Cor", adotam.AnimalFK);
            ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "Id", "CodPostal", adotam.UtilizadorFK);
            return View(adotam);
        }

        // GET: Adotam/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adotam = await _context.Adotam.FindAsync(id);
            if (adotam == null)
            {
                return NotFound();
            }
            ViewData["AnimalFK"] = new SelectList(_context.Animal, "Id", "Cor", adotam.AnimalFK);
            ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "Id", "CodPostal", adotam.UtilizadorFK);
            return View(adotam);
        }

        // POST: Adotam/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,dateA,UtilizadorFK,AnimalFK")] Adotam adotam)
        {
            if (id != adotam.AnimalFK)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adotam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdotamExists(adotam.AnimalFK))
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
            ViewData["AnimalFK"] = new SelectList(_context.Animal, "Id", "Cor", adotam.AnimalFK);
            ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "Id", "CodPostal", adotam.UtilizadorFK);
            return View(adotam);
        }

        // GET: Adotam/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adotam = await _context.Adotam
                .Include(a => a.Animal)
                .Include(a => a.Utilizador)
                .FirstOrDefaultAsync(m => m.AnimalFK == id);
            if (adotam == null)
            {
                return NotFound();
            }

            return View(adotam);
        }

        // POST: Adotam/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var adotam = await _context.Adotam.FindAsync(id);
            if (adotam != null)
            {
                _context.Adotam.Remove(adotam);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdotamExists(int id)
        {
            return _context.Adotam.Any(e => e.AnimalFK == id);
        }
    }
}
