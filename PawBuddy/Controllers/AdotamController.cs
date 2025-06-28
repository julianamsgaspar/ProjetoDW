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
    /// <summary>
    /// Controlador responsável pela gestão das adoções.
    /// Acesso restrito ao perfil "Admin".
    /// </summary>
    [Authorize(Roles = "Admin")] // Só admin acede 
    public class AdotamController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        /// <summary>
        /// Construtor do controlador que recebe o contexto da base de dados.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public AdotamController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todas as adoções registadas, incluindo dados do animal e do utilizador.
        /// </summary>
        /// <returns>Vista com a lista de adoções.</returns>
        // GET: Adotam
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Adotam.Include(a => a.Animal).Include(a => a.Utilizador);
            return View(await applicationDbContext.ToListAsync());
        }
        
        public async Task<IActionResult> IndexPartial()
        {
            var listaAdotam = await _context.Adotam
                .Include(a => a.Utilizador)
                .Include(a => a.Animal)
                .ToListAsync();

            return PartialView("_IndexPartial", listaAdotam);
        }

        /// <summary>
        /// Mostra os detalhes de uma adoção específica.
        /// </summary>
        /// <param name="id">ID do animal (chave estrangeira).</param>
        /// <returns>Vista com os detalhes ou NotFound.</returns>
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
        
        /// <summary>
        /// Exibe o formulário de criação de uma nova adoção.
        /// </summary>
        /// <returns>Vista de criação com dropdowns para selecionar Animal e Utilizador.</returns>
        // GET: Adotam/Create
        public IActionResult Create()
        {
            ViewData["AnimalFK"] = new SelectList(_context.Animal, "Id", "Cor");
            ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "Id", "CodPostal");
            return View();
        }
        
        /// <summary>
        /// Regista uma nova adoção na base de dados.
        /// </summary>
        /// <param name="adotam">Objeto com os dados da adoção.</param>
        /// <returns>Redireciona para o índice se bem-sucedido, caso contrário retorna a vista.</returns>
        // POST: Adotam/Create
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
        
        /// <summary>
        /// Exibe o formulário para editar os dados de uma adoção existente.
        /// </summary>
        /// <param name="id">ID da adoção (animal).</param>
        /// <returns>Vista de edição com os dados preenchidos.</returns>
        // GET: Adotam/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
        
        /// <summary>
        /// Atualiza os dados de uma adoção existente.
        /// </summary>
        /// <param name="id">ID original da adoção.</param>
        /// <param name="adotam">Objeto com os novos dados.</param>
        /// <returns>Redireciona para o índice se bem-sucedido, ou retorna a vista com erros.</returns>
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
        
        /// <summary>
        /// Mostra a confirmação de eliminação de uma adoção.
        /// </summary>
        /// <param name="id">ID do animal relacionado com a adoção.</param>
        /// <returns>Vista de confirmação ou NotFound.</returns>
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
        
        /// <summary>
        /// Elimina permanentemente uma adoção da base de dados.
        /// </summary>
        /// <param name="id">ID da adoção a ser eliminada.</param>
        /// <returns>Redireciona para o índice após a eliminação.</returns>
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
        
        /// <summary>
        /// Verifica se uma adoção existe com base no ID do animal.
        /// </summary>
        /// <param name="id">ID do animal (FK).</param>
        /// <returns>True se existir, false caso contrário.</returns>
        private bool AdotamExists(int id)
        {
            return _context.Adotam.Any(e => e.AnimalFK == id);
        }
    }
}
