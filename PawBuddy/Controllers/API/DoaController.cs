using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawBuddy.Data;
using PawBuddy.Models;

namespace PawBuddy.Controllers.API
{
    /// <summary>
    /// Controlador API para gestão de doações
    /// Permite operações CRUD sobre doações
    /// </summary>
    [Route("api/Doa")] // Rota base para todos os endpoints
    [ApiController]
    [Authorize] // Requer autenticação para todos os endpoints
    public class DoaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor que recebe o contexto da base de dados
        /// </summary>
        /// <param name="context">Contexto de acesso à base de dados</param>
        public DoaController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todas as doações registadas
        /// </summary>
        /// <returns>Lista de doações com informações de animal e utilizador</returns>
        // GET: api/Doa
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doa>>> GetDoa()
        {
            // Inclui informações relacionadas de Animal e Utilizador
            var doacoes = await _context.Doa
                .Include(d => d.Animal)
                .Include(d => d.Utilizador)
                .ToListAsync();

           
            return doacoes;
        }

        /// <summary>
        /// Obtém uma doação específica pelo seu ID
        /// </summary>
        /// <param name="id">ID da doação</param>
        /// <returns>Dados da doação ou NotFound se não existir</returns>
        // GET: api/Doa/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Doa>> GetDoa(int id)
        {
            // Procura a doação incluindo informações relacionadas
            var doa = await _context.Doa
                .Include(d => d.Animal)
                .Include(d => d.Utilizador)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doa == null)
                return NotFound();

            return doa;
        }

        /// <summary>
        /// Cria uma nova doação para um animal específico
        /// </summary>
        /// <param name="id">ID do animal</param>
        /// <param name="doa">Dados da doação</param>
        /// <returns>Dados da doação criada ou erro</returns>
        // POST: api/Doa
        [HttpPost("{id:int}")]
        public async Task<ActionResult<Doa>> PostDoa(int id, [FromBody] Doa doa)
        {
            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(u => u.Nome == User.Identity.Name);

            if (utilizador == null)
                return Unauthorized("Utilizador não encontrado.");

            var animal = await _context.Animal.FindAsync(id);
            if (animal == null)
                return NotFound("Animal não encontrado.");

            // Geração de ID único se necessário
            if (doa.Id <= 0 || await _context.Doa.AnyAsync(d => d.Id == doa.Id))
            {
                int maxId = await _context.Doa.MaxAsync(d => (int?)d.Id) ?? 0;
                doa.Id = maxId + 1;
            }

            // Conversão do valor se fornecido como string
            if (!string.IsNullOrWhiteSpace(doa.PrecoAux))
            {
                doa.Valor = Convert.ToDecimal(doa.PrecoAux.Replace(".", ","), new CultureInfo("pt-PT"));
            }
            
            // Preenchimento dos campos obrigatórios
            doa.AnimalFK = animal.Id;
            doa.UtilizadorFK = utilizador.Id;
            doa.DataD = DateTime.Now;

            // Adiciona e guarda na base de dados
            _context.Doa.Add(doa);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDoa), new { id = doa.Id }, doa);
        }

        /// <summary>
        /// Atualiza uma doação existente
        /// </summary>
        /// <param name="id">ID da doação a atualizar</param>
        /// <param name="doaAtualizada">Dados atualizados da doação</param>
        /// <returns>Resultado da operação</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoa(int id, Doa doaAtualizada)
        {
            // Obter utilizador autenticado
            var utilizadorAtual = await _context.Utilizador
                .FirstOrDefaultAsync(u => u.Nome == User.Identity.Name);

            if (utilizadorAtual == null)
                return Unauthorized("Utilizador não encontrado.");
            // Obter doação existente
            var doacaoExistente = await _context.Doa.FindAsync(id);
            if (doacaoExistente == null)
                return NotFound("Doação não encontrada.");

            // Verificar se o utilizador atual é o dono da doação
            if (doacaoExistente.UtilizadorFK != utilizadorAtual.Id)
                return Forbid("Não tem permissão para modificar esta doação.");

            // Verificar se o animal existe
            if (!await _context.Animal.AnyAsync(a => a.Id == doacaoExistente.AnimalFK))
                return BadRequest("Animal não existe.");

            // Atualizar campos
            if (!string.IsNullOrWhiteSpace(doaAtualizada.PrecoAux))
            {
                doacaoExistente.Valor = Convert.ToDecimal(doaAtualizada.PrecoAux.Replace(".", ","), new CultureInfo("pt-PT"));
            }
            doacaoExistente.UtilizadorFK = doaAtualizada.UtilizadorFK;
            doacaoExistente.DataD = doaAtualizada.DataD;
            doacaoExistente.AnimalFK = doaAtualizada.AnimalFK;
    
            
            
            return NoContent();
        }

        /// <summary>
        /// Verifica se uma doação existe
        /// </summary>
        /// <param name="id">ID da doação</param>
        /// <returns>Verdadeiro se existir, falso caso contrário</returns>
        private bool DoaExiste(int id)
        {
            return _context.Doa.Any(e => e.Id == id);
        }

        /// <summary>
        /// Elimina uma doação existente
        /// </summary>
        /// <param name="id">ID da doação a eliminar</param>
        /// <returns>Resultado da operação</returns>
        // DELETE: api/Doa/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoa(int id)
        {
            var doa = await _context.Doa.FindAsync(id);
            if (doa == null)
                return NotFound();
            _context.Doa.Remove(doa);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Verifica se uma doação existe
        /// </summary>
        /// <param name="id">ID da doação</param>
        /// <returns>Verdadeiro se existir, falso caso contrário</returns>
        private bool DoaExists(int id)
        {
            return _context.Doa.Any(e => e.Id == id);
        }
    }
}
