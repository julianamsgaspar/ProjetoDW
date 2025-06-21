using System;
using System.Collections.Generic;
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
    /// Controlador responsável por gerir as intenções de adoção submetidas pelos utilizadores.
    /// Acesso restrito a utilizadores com o papel de "Admin".
    /// </summary>
    //[Authorize(Roles = "Admin")] 

    public class IntencaoDeAdocaoController : Controller
    
    {
        // Contexto da base de dados
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor que recebe o contexto da base de dados
        /// </summary>
        /// <param name="context">Contexto da base de dados</param>
        public IntencaoDeAdocaoController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Lista todas as intenções de adoção existentes.
        /// </summary>
        // GET: intencaoDeAdocao
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Obter todas as intenções incluindo informações de Animal e Utilizador
            var applicationDbContext = _context.Intencao
                .Include(i => i.Animal)
                .Include(i => i.Utilizador);
            return View(await applicationDbContext.ToListAsync());
        }

        /// <summary>
        /// Mostra os detalhes de uma intenção de adoção específica
        /// </summary>
        /// <param name="id">ID da intenção</param>
        // GET: intencaoDeAdocao/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Verificar se o ID foi fornecido
            if (id == null)
            {
                return NotFound();
            }

            // Obter intenção com informações relacionadas
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
        /// Mostra o formulário para criar nova intenção de adoção
        /// </summary>
        /// <param name="id">ID do animal a ser adotado</param>
        /// <returns>View de criação ou NotFound</returns>
        // GET: intencaoDeAdocao/Create
        [HttpGet]
        public async Task<IActionResult> Create( int id)
        {
            // Validar ID do animal
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            // Passar dados para a view
            ViewBag.AnimalId = id;
            var animal = await _context.Animal.Where(u => u.Id == id).FirstOrDefaultAsync();
            ViewBag.Animal = animal;
            return View();
            
         
        }

        /// <summary>
        /// Processa a criação de uma nova intenção de adoção
        /// </summary>
        /// <param name="id">ID do animal</param>
        // POST: intencaoDeAdocao/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( int id, [Bind("Estado,temAnimais, quaisAnimais,Profissao,Residencia,Motivo,DataIA,AnimalFK")] IntencaoDeAdocao intencaoDeAdocao)
        {
            // Obter ID do utilizador autenticado
            int idUser;
            idUser = await _context.Utilizador
                .Where(u => User.Identity.Name == u.Nome)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (idUser == null)
            {
                return NotFound();
            }
            /// valida as chaves forasteiras
            if (id != intencaoDeAdocao.AnimalFK)
            {
                return NotFound();
            }
            
            // Verificar se animal existe
            int animalId = await _context.Animal.Where(u => u.Id == id)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();
            if (animalId == null)
            {
                return NotFound();
            }
            
            // Verificar se já existe alguma intenção para este animal
            var intencoesExistentes = await _context.Intencao
                .Where(i => i.AnimalFK == id && i.Estado != EstadoAdocao.Rejeitado)
                .ToListAsync();
            // Verificar se animal já está em processo de adoção ou foi adotado
            var statusBloqueadores = new List<EstadoAdocao> { EstadoAdocao.Emvalidacao, EstadoAdocao.Concluido };
    
            var animalIndisponivelIntencao = await _context.Intencao
                .AnyAsync(i => i.AnimalFK == id && statusBloqueadores.Contains(i.Estado));
            var animalIndisponivelAdotam = await _context.Adotam.AnyAsync(i => i.AnimalFK == id);
            
            
            if (animalIndisponivelIntencao || animalIndisponivelAdotam)
            {
                ModelState.AddModelError(string.Empty, "Este animal já está em processo de adoção ou foi adotado e não está disponível.");
                //return View(intencaoDeAdocao);
            }
    
            // Verificar se utilizador já manifestou interesse neste animal
            var intencaoUsuarioExistente = await _context.Intencao
                .AnyAsync(i => i.AnimalFK == id && i.UtilizadorFK == idUser);
    
            if (intencaoUsuarioExistente)
            {
                ModelState.AddModelError(string.Empty, "Você já manifestou interesse em adotar este animal.");
                //return View(intencaoDeAdocao);
            }
            
            
            //o utilizador nao pode adotar o mesmo animal 2 vezes 
            if (ModelState.IsValid)
           {
               // Preencher dados da intenção
               intencaoDeAdocao.UtilizadorFK = idUser;
               intencaoDeAdocao.AnimalFK = animalId;
               // Forçar o estado para "Reservado"
               intencaoDeAdocao.Estado = EstadoAdocao.Reservado;
               // Mete a data sem mostrar ao utilizador, mete a data quando submete o "formulario"
               intencaoDeAdocao.DataIA = DateTime.Now;
               // Definir estado inicia
               
               if (intencoesExistentes.Any())
               {
                   // Se já existem intenções, marcar todas como Emprocesso
                   foreach (var existente in intencoesExistentes)
                   {
                       existente.Estado = EstadoAdocao.Emprocesso;
                       _context.Update(existente);
                   }
                   intencaoDeAdocao.Estado = EstadoAdocao.Emprocesso;
               }
               else
               {
                   // Primeira intenção para este animal
                   intencaoDeAdocao.Estado = EstadoAdocao.Emvalidacao;
               }
               // Guardar na base de dados
               ViewBag.Animal = animalId;
               _context.Add(intencaoDeAdocao);
               await _context.SaveChangesAsync();
               return RedirectToAction(nameof(Index));
           }
         //ViewData["AnimalFK"] = new SelectList(_context.Animal, "Id", "Nome", intencaoDeAdocao.AnimalFK);
           //ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "Id", "Nome", intencaoDeAdocao.UtilizadorFK);
           return View(intencaoDeAdocao);
       }

        /// <summary>
        /// Mostra formulário para editar intenção de adoção
        /// </summary>
        /// <param name="id">ID da intenção</param>
        /// <returns>View de edição ou NotFound</returns>
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

           // Remove os SelectLists desnecessários
           return View(intencaoDeAdocao);
       }

        /// <summary>
        /// Processa a edição de uma intenção de adoção
        /// </summary>
        /// <param name="id">ID da intenção</param>
        /// <param name="intencaoDeAdocao">Dados atualizados</param>
        /// <returns>Redireciona para Index ou mostra erros</returns>
       // POST: intencaoDeAdocao/Edit/5
       // To protect from overposting attacks, enable the specific properties you want to bind to.
       // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
       [HttpPost]
       [ValidateAntiForgeryToken]
       public async Task<IActionResult> Edit(int id, [Bind("Id,Estado,temAnimais,quaisAnimais,Profissao,Residencia,Motivo,UtilizadorFK,AnimalFK, DataIA")] IntencaoDeAdocao intencaoDeAdocao)
       {
           // Verificar se o ID corresponde
           if (id != intencaoDeAdocao.Id)
           {
               return NotFound();
           }


           // Verificar se utilizador e animal existem
           var utilizador = await _context.Utilizador.FindAsync(intencaoDeAdocao.UtilizadorFK);
           var animal = await _context.Animal.FindAsync(intencaoDeAdocao.AnimalFK);
            
           if (utilizador == null || animal == null)
           {
               ModelState.AddModelError("", "Utilizador ou Animal não encontrado.");
               return View(intencaoDeAdocao);
           }
           // Obter intenção existente
           var intencaoExistente = await _context.Intencao.FindAsync(id);

           if (intencaoExistente == null){
               return NotFound(); }

           if (ModelState.IsValid)
           {
               try
               {
                

                   // Atualizar apenas campos permitidos
                   intencaoExistente.Estado = intencaoDeAdocao.Estado;
                   intencaoExistente.temAnimais = intencaoDeAdocao.temAnimais;
                   intencaoExistente.quaisAnimais = intencaoDeAdocao.quaisAnimais;
                   intencaoExistente.Profissao = intencaoDeAdocao.Profissao;
                   intencaoExistente.Residencia = intencaoDeAdocao.Residencia;
                   intencaoExistente.Motivo = intencaoDeAdocao.Motivo;
                   intencaoExistente.DataIA = intencaoDeAdocao.DataIA;

                   // Mantém as chaves estrangeiras originais
                   intencaoExistente.UtilizadorFK = intencaoDeAdocao.UtilizadorFK;
                   intencaoExistente.AnimalFK = intencaoDeAdocao.AnimalFK;
                   
                   // Processar estado da intenção
                   if (intencaoDeAdocao.Estado == EstadoAdocao.Concluido)
                   {
                       // Verificar se animal já foi adotado
                       bool animalJaAdotado = await _context.Adotam
                           .AnyAsync(a => a.AnimalFK == intencaoExistente.AnimalFK);

                       if (!animalJaAdotado)
                       {
                           // Criar registo de adoção
                           var finalAdotam = new Adotam();
                           finalAdotam.AnimalFK = intencaoExistente.AnimalFK;
                           finalAdotam.UtilizadorFK = intencaoExistente.UtilizadorFK;
                           finalAdotam.dateA = DateTime.Now;
                           _context.Adotam.Add(finalAdotam);
                       }
                       // Remover todas as intenções para este animal
                       var outrasIntencoes = await _context.Intencao
                               .Where(i => i.AnimalFK == intencaoExistente.AnimalFK)
                               .ToListAsync(); 
                       _context.Intencao.RemoveRange(outrasIntencoes);
                       
                       
                   }
                   else if (intencaoDeAdocao.Estado == EstadoAdocao.Rejeitado)
                   {
                    // Apenas marcar como rejeitado (pode ser reconsiderado)
                       _context.Intencao.Remove(intencaoExistente);
                   }
                   // Salva todas as alterações no banco de dados
                   await _context.SaveChangesAsync();
            
                   // Redireciona para a lista após edição bem-sucedida
                   return RedirectToAction(nameof(Index));

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
           }
           return View(intencaoExistente);
       }

       /// <summary>
       /// Mostra o formulário de confirmação para apagar uma intenção de adoção.
       /// </summary>
       /// <param name="id">ID da intenção</param>
       /// <returns>View de confirmação ou NotFound</returns>
       // GET: intencaoDeAdocao/Delete/5
       public async Task<IActionResult> Delete(int? id)
       {
           if (id == null)
           {
               return NotFound();
           }

           // Obter intenção com informações relacionadas
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
       ///  <param name="id">ID da intenção</param>
       /// <returns>Redireciona para Index</returns>
       // POST: intencaoDeAdocao/Delete/5
       [HttpPost]
       [ValidateAntiForgeryToken]
       public async Task<IActionResult> DeleteConfirmed(int id)
       {
         
           var intencaoDeAdocao = await _context.Intencao
               .Include(i => i.Animal)
               .Include(i => i.Utilizador)
               .FirstOrDefaultAsync(i => i.Id == id);
    
           if (intencaoDeAdocao == null)
           {
               return NotFound();
           }

           // Validar ID da sessão
           var idSessao = HttpContext.Session.GetInt32("idSessao");
           if (idSessao == null || idSessao != id)
           {
               return RedirectToAction(nameof(Details), new { id = id });
           }

           try
           {
               // Eliminar intenção
               _context.Intencao.Remove(intencaoDeAdocao);
               await _context.SaveChangesAsync();
        
               // Limpar sessão
               HttpContext.Session.Remove("idSessao");
        
               return RedirectToAction(nameof(Index));
           }
           catch (DbUpdateException ex)
           {
          
               ModelState.AddModelError("", "Não foi possível eliminar. Tente novamente, e se o problema persistir contacte o administrador.");
               return View("Delete", intencaoDeAdocao);
           }

       }

       /// <summary>
       /// Verifica se uma intenção de adoção com o ID especificado existe na base de dados.
       /// </summary>
       /// <param name="id">ID da intenção</param>
       /// <returns>True se existir, False caso contrário</returns>
       private bool IntencaoDeAdocaoExists(int id)
       {
           return _context.Intencao.Any(e => e.Id == id);
       }
   }
}
