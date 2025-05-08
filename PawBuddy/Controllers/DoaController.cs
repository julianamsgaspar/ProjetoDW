using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class DoaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Doa
        public async Task<IActionResult> Index()
        {
            var ListaDeDoacoes = _context.Doa
                .Include(d => d.Animal).
                Include(d => d.Utilizador);
            return View(await ListaDeDoacoes.ToListAsync());
        }

        // GET: Doa/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doa = await _context.Doa
                .Include(d => d.Animal)
                .Include(d => d.Utilizador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doa == null)
            {
                return NotFound();
            }

            return View(doa);
        }

        // GET: Doa/Create
        public IActionResult Create()
        {
            ViewData["AnimalFK"] = new SelectList(_context.Animal, "Id", "Nome");
            ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "Id", "Nome");
            return View();
        }

        // POST: Doa/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PrecoAux,DataD,UtilizadorFK,AnimalFK")] Doa doa)
        {
            doa.DataD = DateTime.Now;
            var utilizador = _context.Utilizador.
                Where(u=> u.Id == doa.UtilizadorFK);
            if (!utilizador.Any())
            {
                ModelState.AddModelError("UtilizadorFK", "Tem de selecionar um utilizador correto");
            }

            if (ModelState.IsValid)
            {
                doa.Valor = Convert.ToDecimal(doa.PrecoAux.Replace(".", ","), 
                    new CultureInfo("pt-PT"));
                
                _context.Add(doa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AnimalFK"] = new SelectList(_context.Animal, "Id", "Nome", doa.AnimalFK);
            ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "Id", "Nome", doa.UtilizadorFK);
            
            return View(doa);
        }

        // GET: Doa/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doa = await _context.Doa.FindAsync(id);
            if (doa == null)
            {
                return NotFound();
            }
            ViewData["AnimalFK"] = new SelectList(_context.Animal, "Id", "Nome", doa.AnimalFK);
            ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "Id", "Nome", doa.UtilizadorFK);
            return View(doa);
        }

        // POST: Doa/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Valor,DataD,UtilizadorFK,AnimalFK")] Doa doa)
        {
            if (id != doa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoaExists(doa.Id))
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
            ViewData["AnimalFK"] = new SelectList(_context.Animal, "Id", "Nome", doa.AnimalFK);
            ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "Id", "Nome", doa.UtilizadorFK);
            return View(doa);
        }

        // GET: Doa/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doa = await _context.Doa
                .Include(d => d.Animal)
                .Include(d => d.Utilizador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doa == null)
            {
                return NotFound();
            }

            return View(doa);
        }

        // POST: Doa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doa = await _context.Doa.FindAsync(id);
            if (doa != null)
            {
                _context.Doa.Remove(doa);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoaExists(int id)
        {
            return _context.Doa.Any(e => e.Id == id);
        }
    }
}
