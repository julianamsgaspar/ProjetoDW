using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PawBuddy.Data;
using PawBuddy.Models;

namespace PawBuddy.Controllers
{
    /// <summary>
    /// Controller responsável pela Intençao dos utilizadores em adotar os animais 
    /// </summary>
    public class IntencaoDeAdocaoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IntencaoDeAdocaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: intencaoDeAdocao
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Intencao
                .Include(i => i.Animal)
                .Include(i => i.Utilizador);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: intencaoDeAdocao/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var intencaoDeAdocao = await _context.Intencao
                .Include(i => i.Animal)
                .Include(i => i.Utilizador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (intencaoDeAdocao == null)
            {
                return NotFound();
            }

            return View(intencaoDeAdocao);
        }

        // GET: intencaoDeAdocao/Create
        public IActionResult Create()
        {
            ViewData["AnimalFK"] = new SelectList(_context.Animal, "Id", "Nome");
            ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "Id", "Nome");
            return View();
        }

        // POST: intencaoDeAdocao/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Estado,temAnimais, quaisAnimais,Profissao,Residencia,Motivo,DataIA,UtilizadorFK,AnimalFK")] IntencaoDeAdocao intencaoDeAdocao, List<int> animaisAAdotar)
        {
            // Forçar o estado para "Reservado"
            intencaoDeAdocao.Estado = EstadoAdocao.Reservado;
            // Mete a data sem mostrar ao utilizador, mete a data quando submete o "formulario"
            intencaoDeAdocao.DataIA = DateTime.Now;
            //foreach (var id in animaisAAdotar )
            //{
                //Animal animal = _context.Animal.Find(id);
               
                
           // }
            
            
            if (ModelState.IsValid)
            {
                
                _context.Add(intencaoDeAdocao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AnimalFK"] = new SelectList(_context.Animal, "Id", "Nome", intencaoDeAdocao.AnimalFK);
            ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "Id", "Nome", intencaoDeAdocao.UtilizadorFK);
            return View(intencaoDeAdocao);
        }

        // GET: intencaoDeAdocao/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var intencaoDeAdocao = await _context.Intencao.FindAsync(id);
            if (intencaoDeAdocao == null)
            {
                return NotFound();
            }
            ViewData["AnimalFK"] = new SelectList(_context.Animal, "Id", "Nome", intencaoDeAdocao.AnimalFK);
            ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "Id", "Nome", intencaoDeAdocao.UtilizadorFK);
            return View(intencaoDeAdocao);
        }

        // POST: intencaoDeAdocao/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Estado,temAnimais, temAnimais,Profissao,Residencia,Motivo,DataIA,UtilizadorFK,AnimalFK")] IntencaoDeAdocao intencaoDeAdocao)
        {
            if (id != intencaoDeAdocao.Id)
            {
                return NotFound();
            }

            // vou buscar o id da intenção
            var idSessao = HttpContext.Session.GetInt32("idSessao");
            // se o id da intenção da sessão for diferente do que recebemos
            // quer dizer que está a tentar alterar um id da intenção são diferente do que tem no ecrã
            if (idSessao != id)
            {
                ModelState.AddModelError("", "Erro: Não tens permissão");
                return View(intencaoDeAdocao);
                        
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    
                    _context.Update(intencaoDeAdocao);
                    await _context.SaveChangesAsync();
                    
                    // colocamos o Idsessao da sessão a 0, para ele não poder fazer POSTs sucessivos 
                    HttpContext.Session.SetInt32("idSessao", 0);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IntencaoDeAdocaoExists(intencaoDeAdocao.Id))
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
            ViewData["AnimalFK"] = new SelectList(_context.Animal, "Id", "Nome", intencaoDeAdocao.AnimalFK);
            ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "Id", "Nome", intencaoDeAdocao.UtilizadorFK);
            return View(intencaoDeAdocao);
        }

        // GET: intencaoDeAdocao/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var intencaoDeAdocao = await _context.Intencao
                .Include(i => i.Animal)
                .Include(i => i.Utilizador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (intencaoDeAdocao == null)
            {
                return NotFound();
            }

            // guardamos em sessão o id da intensao que o utilizador quer apagar
            // se ele fizer um post para um Id diferente, ele está a tentar apagar um utilizador diferente do que visualiza no ecrã
            HttpContext.Session.SetInt32("idSessao", intencaoDeAdocao.Id);
            return View(intencaoDeAdocao);
        }

        // POST: intencaoDeAdocao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var intencaoDeAdocao = await _context.Intencao.FindAsync(id);
            if (intencaoDeAdocao != null)
            {
                // vou buscar o id do utilizador da sessão
                var idSessao = HttpContext.Session.GetInt32("idSessao");
                // se o id do utilizador da sessão for diferente do que recebemos
                // quer dizer que está a tentar apagar um utilizador diferente do que tem no ecrã
                if (idSessao != id)
                {
                    return RedirectToAction(nameof(Index));
                }
                _context.Intencao.Remove(intencaoDeAdocao);
            }
 
            await _context.SaveChangesAsync();
            // impede que tente fazer o apagar do mesmo utilizador
            HttpContext.Session.SetInt32("idSessao",0);
            return RedirectToAction(nameof(Index));
        }

        private bool IntencaoDeAdocaoExists(int id)
        {
            return _context.Intencao.Any(e => e.Id == id);
        }
    }
}
