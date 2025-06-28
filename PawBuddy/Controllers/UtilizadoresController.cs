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
    /// Controller responsável pela gestão dos Utilizadores.
    /// Apenas acessível a utilizadores com o perfil de Admin.
    /// </summary>
    [Authorize(Roles = "Admin")] // Só admin acede 
    public class UtilizadoresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UtilizadoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todos os utilizadores registados.
        /// </summary>
        // GET: Utilizadores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Utilizador.ToListAsync());
        }
        public IActionResult IndexPartial()
        {
            var utilizadores = _context.Utilizador.ToList();
            return PartialView("_IndexPartial", utilizadores);
        }

        /// <summary>
        /// Mostra os detalhes de um utilizador específico.
        /// </summary>
        // GET: Utilizadores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilizador == null)
            {
                return NotFound();
            }

            return View(utilizador);
        }

        // <summary>
        /// Exibe o formulário para criação de um novo utilizador.
        /// </summary>
        // GET: Utilizadores/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Regista um novo utilizador na base de dados.
        /// </summary>
        // POST: Utilizadores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,DataNascimento,Nif,Telemovel,Morada,CodPostal,Email,Pais")] Utilizador utilizador)
        {
            if (ModelState.IsValid)
            {
                _context.Add(utilizador);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(utilizador);
        }

        /// <summary>
        /// Exibe o formulário de edição para um utilizador existente.
        /// </summary>
        // GET: Utilizadores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizador.FindAsync(id);
           
            if (utilizador == null)
            {
                return NotFound();
            } 
            // Guarda o ID do utilizador em sessão para validações de segurança posteriores
            // se ele fizer um post para um Id diferente, ele está a tentar alterar um utilizador diferente do que visualiza no ecrã
            HttpContext.Session.SetInt32("utilizadorId", utilizador.Id);
                         
            return View(utilizador);
        }

        /// <summary>
        /// Atualiza os dados de um utilizador, após validação da sessão.
        /// </summary>
        // POST: Utilizadores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute]int id, [Bind("Id,DataNascimento,Nome,Nif,Telemovel,Morada,CodPostal,Email,Pais")] Utilizador utilizador)
        {
            if (id != utilizador.Id)
            {
                return NotFound();
            }
            
            
            
            if (ModelState.IsValid)
            {
                try
                {
                    // vou buscar o id do utilizador da sessão
                    var utilizadorDaSessao = HttpContext.Session.GetInt32("utilizadorId");
                    // Verifica se o utilizador que está a tentar editar é o mesmo que estava na tela
                    if (utilizadorDaSessao != id)
                    {
                        ModelState.AddModelError("", "Erro: Não tens permissão");
                        return View(utilizador);
                    }
                    _context.Update(utilizador);
                    await _context.SaveChangesAsync();
                    // Limpa o ID da sessão para evitar edições duplicadas 
                    HttpContext.Session.SetInt32("utilizadorId", 0);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilizadorExists(utilizador.Id))
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
            return View(utilizador);
        }

        /// <summary>
        /// Exibe o formulário de confirmação para eliminar um utilizador.
        /// </summary>
        // GET: Utilizadores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilizador == null)
            {
                return NotFound();
            }

            // Guarda o ID em sessão para validação posterior
            // se ele fizer um post para um Id diferente, ele está a tentar apagar um utilizador diferente do que visualiza no ecrã
            HttpContext.Session.SetInt32("utilizadorId", utilizador.Id);

            return View(utilizador);
        }

        /// <summary>
        /// Apaga o utilizador da base de dados após confirmar a identidade via sessão.
        /// </summary>
        // POST: Utilizadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            
            var utilizador = await _context.Utilizador.FindAsync(id);
            if (utilizador != null)
            {
                // vou buscar o id do utilizador da sessão
                var utilizadorDaSessao = HttpContext.Session.GetInt32("utilizadorId");
                // Verifica se é o mesmo utilizador que estava a ser exibido no momento da confirmação
                // quer dizer que está a tentar apagar um utilizador diferente do que tem no ecrã
                if (utilizadorDaSessao != id)
                {
                    return RedirectToAction(nameof(Index));
                }
                _context.Utilizador.Remove(utilizador);
            }

            await _context.SaveChangesAsync();
            // Limpa o ID da sessão para prevenir exclusões repetidas
            HttpContext.Session.SetInt32("utilizadorId",0);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se um utilizador existe na base de dados com base no seu ID.
        /// </summary>
        private bool UtilizadorExists(int id)
        {
            return _context.Utilizador.Any(e => e.Id == id);
        }
    }
}
