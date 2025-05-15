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
    /// Controlador responsável por gerir as intenções de adoção submetidas pelos utilizadores.
    /// Acesso restrito a utilizadores com o papel de "Admin".
    /// </summary>
    [Authorize(Roles = "Admin")] 
    public class IntencaoDeAdocaoController : Controller
    
    {
        private readonly ApplicationDbContext _context;

        public IntencaoDeAdocaoController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Lista todas as intenções de adoção existentes.
        /// </summary>
        // GET: intencaoDeAdocao
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Intencao
                .Include(i => i.Animal)
                .Include(i => i.Utilizador);
            return View(await applicationDbContext.ToListAsync());
        }

        /// <summary>
        /// Mostra os detalhes de uma intenção de adoção específica.
        /// </summary>
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

        /// <summary>
        /// Apresenta o formulário para criar uma nova intenção de adoção.
        /// </summary>
        // GET: intencaoDeAdocao/Create
        public IActionResult Create()
        {
            ViewData["AnimalFK"] = new SelectList(_context.Animal, "Id", "Nome");
            ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "Id", "Nome");
            return View();
        }

        /// <summary>
        /// Processa os dados do formulário de criação de uma nova intenção de adoção.
        /// </summary>
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

        /// <summary>
        /// Apresenta o formulário de edição para uma intenção de adoção.
        /// </summary>
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

        /// <summary>
        /// Processa os dados da edição de uma intenção de adoção.
        /// Verifica se o ID da sessão corresponde ao da intenção.
        /// </summary>
        // POST: intencaoDeAdocao/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Estado,temAnimais,Profissao,Residencia,Motivo,DataIA,UtilizadorFK,AnimalFK")] IntencaoDeAdocao intencaoDeAdocao)
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

        /// <summary>
        /// Mostra o formulário de confirmação para apagar uma intenção de adoção.
        /// </summary>
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

        /// <summary>
        /// Confirmação da exclusão da intenção de adoção.
        /// Verifica se o ID corresponde ao da sessão.
        /// </summary>
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

        /// <summary>
        /// Verifica se uma intenção de adoção com o ID especificado existe na base de dados.
        /// </summary>
        private bool IntencaoDeAdocaoExists(int id)
        {
            return _context.Intencao.Any(e => e.Id == id);
        }
    }
}
