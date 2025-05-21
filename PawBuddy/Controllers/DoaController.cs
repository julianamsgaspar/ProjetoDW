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
    /// <summary>
    /// Controlador responsável pela gestão de doações. 
    /// Apenas utilizadores com o perfil "Admin" têm acesso.
    /// </summary>
    //[Authorize(Roles = "Admin")] // Só admin acessa 
    public class DoaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoaController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Apresenta a lista de todas as doações registadas no sistema.
        /// </summary>
        /// <returns>View com a lista de doações.</returns>
        // GET: Doa
        public async Task<IActionResult> Index()
        {
            var ListaDeDoacoes = _context.Doa
                .Include(d => d.Animal).
                Include(d => d.Utilizador);
            return View(await ListaDeDoacoes.ToListAsync());
        }
    
        /// <summary>
        /// Apresenta os detalhes de uma doação específica.
        /// </summary>
        /// <param name="id">Identificador da doação.</param>
        /// <returns>View com os detalhes da doação, ou NotFound se não existir.</returns>
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

        /// <summary>
        /// Apresenta o formulário para criar uma nova doação.
        /// </summary>
        /// <returns>View de criação da doação.</returns>
        // GET: Doa/Create
        public IActionResult Create()
        {
            ViewData["AnimalFK"] = new SelectList(_context.Animal, "Id", "Nome");
            ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "Id", "Nome");
            return View();
        }

        // <summary>
        /// Processa a criação de uma nova doação.
        /// </summary>
        /// <param name="doa">Objeto com os dados da doação.</param>
        /// <returns>Redireciona para o Index em caso de sucesso, senão retorna a mesma view com mensagens de erro.</returns>
        // POST: Doa/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PrecoAux,DataD,UtilizadorFK,AnimalFK")] Doa doa)
        {
            
            /*var utilizador = _context.Utilizador.
                Where(u=> u.Id == doa.UtilizadorFK);
            var animal = _context.Animal.
                Where(u => u.Id == doa.AnimalFK);
            
            if (!utilizador.Any())
            {
                ModelState.AddModelError("UtilizadorFK", "Utilizador Inválido");
                         
                if (!animal.Any())
                {
                    
                       ModelState.AddModelError("AnimalFK", "Animal Inválido");
                                
                    
                }
            }*/
            doa.Valor = Convert.ToDecimal(doa.PrecoAux.Replace(".", ","), 
                new CultureInfo("pt-PT"));
                
            doa.DataD = DateTime.Now;


            if (ModelState.IsValid)
            {
                
                
                _context.Add(doa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AnimalFK"] = new SelectList(_context.Animal, "Id", "Nome", doa.AnimalFK);
            ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "Id", "Nome", doa.UtilizadorFK);
            
            return View(doa);
        }

        /// <summary>
        /// Apresenta o formulário de edição de uma doação.
        /// </summary>
        /// <param name="id">Identificador da doação.</param>
        /// <returns>View de edição ou NotFound se não existir.</returns>
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
        
        /// <summary>
        /// Processa a atualização das informações de uma doação.
        /// </summary>
        /// <param name="id">Identificador da doação.</param>
        /// <param name="doa">Objeto atualizado da doação.</param>
        /// <returns>Redireciona para Index se for bem-sucedido, senão retorna a mesma view com mensagens de erro.</returns>
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

            
            // Verifica se o animal e utilizador existem
            var utilizador = await _context.Utilizador.FindAsync(doa.UtilizadorFK);
            var animal = await _context.Animal.FindAsync(doa.AnimalFK);
    
            if (utilizador == null || animal == null)
            {
                ModelState.AddModelError("", "Utilizador ou Animal não encontrado.");
                return View(doa);
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    var doacaoExistente= await _context.Doa.FindAsync(id);
            
                    if (doacaoExistente == null)
                    {
                        return NotFound();
                    }

                    // Atualiza apenas os campos permitidos
                    doacaoExistente.Valor = doa.Valor;
                    

                    // Mantém as chaves estrangeiras originais
                    doacaoExistente.UtilizadorFK = doa.UtilizadorFK;
                    doacaoExistente.AnimalFK = doa.AnimalFK;

                    _context.Update(doacaoExistente);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                    
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
        
        /// <summary>
        /// Apresenta a confirmação para eliminar uma doação.
        /// </summary>
        /// <param name="id">Identificador da doação.</param>
        /// <returns>View com os dados da doação ou NotFound se não existir.</returns>
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

        /// <summary>
        /// Confirma e executa a eliminação da doação do sistema.
        /// </summary>
        /// <param name="id">Identificador da doação a eliminar.</param>
        /// <returns>Redireciona para a página Index após a eliminação.</returns>
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

        /// <summary>
        /// Verifica se uma doação com o ID especificado existe na base de dados.
        /// </summary>
        /// <param name="id">ID da doação.</param>
        /// <returns><c>true</c> se existir; caso contrário, <c>false</c>.</returns>
        private bool DoaExists(int id)
        {
            return _context.Doa.Any(e => e.Id == id);
        }
    }
}
