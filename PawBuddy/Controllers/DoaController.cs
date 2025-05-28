using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    [Authorize] 
    //[Route("Doa")]
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
            
            decimal somaValores = ListaDeDoacoes.Sum(d => d.Valor);

            ViewBag.SomaValores = somaValores;
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
        [HttpGet]
        public async Task<IActionResult>  Create( int id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            ViewBag.AnimalId = id;
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
        public async Task<IActionResult> Create([FromRoute] int id, [Bind("Id,PrecoAux,DataD,AnimalFK")] Doa doa)
        {
            
            int idUser;
            idUser = await _context.Utilizador
                .Where(u => User.Identity.Name == u.Nome)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();
            if (idUser == null)
            {
                return NotFound();
            }
            if (id != doa.AnimalFK)
            {
                return NotFound();
            }
            
            int animal = await _context.Animal.Where(u => u.Id == id)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();
            if (animal == null)
            {
                return NotFound();
            }
            
            // Foi verificado que a BD mete o id do animal como id da doação entao foi foram feitos ajustos para ter chaves unicas
            // 1. Verifica se o objeto doa tem um ID válido (0 ou null)
            if (doa.Id <= 0) // Simplifica a verificação (cobre null, 0 e negativos)
            {
                // Se não tiver ID válido:
                // Busca o maior ID existente na tabela Doa de forma segura
                int maxId = await _context.Doa
                    .AsNoTracking() // Melhora performance (apenas leitura)
                    .MaxAsync(d => (int?)d.Id) ?? 0; // Trata tabela vazia
    
                // Garante que o novo ID seja único
                doa.Id = maxId + 1;
            }
            else // 2. Se o objeto doa já tem um ID definido
            {
                // Verifica se o ID já existe na base de dados
                bool idExists = await _context.Doa
                    .AsNoTracking()
                    .AnyAsync(d => d.Id == doa.Id);
    
                // Se o ID já existe, substitui por um novo ID único
                if (idExists)
                {
                    // Busca o próximo ID disponível de forma atômica
                    int nextAvailableId = await _context.Doa
                        .AsNoTracking()
                        .MaxAsync(d => (int?)d.Id) ?? 0;
        
                    doa.Id = nextAvailableId + 1;
        
                }
    
                // Se o ID não existe, mantém o ID fornecido (já é único)
            }
            doa.Valor = Convert.ToDecimal(doa.PrecoAux.Replace(".", ","), 
                new CultureInfo("pt-PT"));
            if (ModelState.IsValid)
            {
                doa.AnimalFK = animal;
                doa.UtilizadorFK = idUser;
                doa.DataD = DateTime.Now;
                _context.Add(doa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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

            HttpContext.Session.SetInt32("idSessao", doa.Id);
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
            {// vou buscar o id do utilizador da sessão
                var idSessao = HttpContext.Session.GetInt32("idSessao");
                // se o id do utilizador da sessão for diferente do que recebemos
                // quer dizer que está a tentar apagar um utilizador diferente do que tem no ecrã
                if (idSessao != id)
                {
                    return RedirectToAction(nameof(Details));
                }
                
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
