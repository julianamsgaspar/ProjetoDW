using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawBuddy.Data;
using PawBuddy.Models;

namespace PawBuddy.Controllers.API
{
    /// <summary>
    /// Controlador API para gestão de animais
    /// Disponibiliza endpoints CRUD para operações com animais
    /// </summary
    [Route("api/Animal")]// Define a rota base para todos os endpoints
    [ApiController] // Indica que é um controlador API
    public class AnimalController : ControllerBase
    {
        // Contexto da base de dados
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor que recebe o contexto da base de dados por injeção de dependência
        /// </summary>
        /// <param name="context">Contexto da base de dados</param>
        public AnimalController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém a lista de todos os animais
        /// </summary>
        /// <returns>Lista de animais</returns>
        // GET: api/Animal
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAnimal()
        {
            return await _context.Animal.ToListAsync();
        }

        /// <summary>
        /// Obtém um animal específico pelo seu ID
        /// </summary>
        /// <param name="id">ID do animal</param>
        /// <returns>Dados do animal ou NotFound se não existir</returns>
        // GET: api/Animal/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Animal>> GetAnimal(int id)
        {
            // Procura o animal na base de dados
            var animal = await _context.Animal.FindAsync(id);

            if (animal == null)
            {
                return NotFound();
            }

            return animal;
        }

        /// <summary>
        /// Cria um novo registo de animal
        /// </summary>
        /// <param name="animal">Dados do animal</param>
        /// <param name="imagem">Ficheiro de imagem do animal</param>
        /// <returns>Dados do animal criado ou erro de validação</returns>
        // POST: api/Animal
        [HttpPost]
        public async Task<ActionResult<Animal>> PostAnimal(  [FromForm] Animal animal, IFormFile imagem)
        {
            // Valida se foi enviada uma imagem
            if (imagem == null || imagem.Length == 0)
            {
                return BadRequest("Imagem é obrigatória.");
            }

            // Define as extensões e tipos MIME permitidos
            var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var mimeTypesValidos = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp" };
            var extensao = Path.GetExtension(imagem.FileName).ToLower();

            // Valida a extensão e tipo MIME do ficheiro
            if (!extensoesPermitidas.Contains(extensao) || !mimeTypesValidos.Contains(imagem.ContentType.ToLower()))
            {
                return BadRequest("O arquivo deve ser uma imagem válida.");
            }

            // Guarda o animal na base de dados para gerar o ID
            _context.Animal.Add(animal);
            await _context.SaveChangesAsync();

            // Prepara o diretório para guardar a imagem
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Gera o nome do ficheiro usando o ID do animal
            var nomeArquivo = $"{animal.Id}{extensao}";
            var caminhoArquivo = Path.Combine(uploadPath, nomeArquivo);

            // Guarda o ficheiro no servidor
            using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
            {
                await imagem.CopyToAsync(stream);
            }

            // Atualiza o caminho da imagem no objeto animal
            animal.Imagem = "/images/" + nomeArquivo;
            _context.Update(animal);
            await _context.SaveChangesAsync();

            // Retorna o animal criado com o status 201 (Created)
            return CreatedAtAction(nameof(GetAnimal), new { id = animal.Id }, animal);
        }

        /// <summary>
        /// Atualiza os dados de um animal existente
        /// </summary>
        /// <param name="id">ID do animal a atualizar</param>
        /// <param name="animal">Dados atualizados do animal</param>
        /// <param name="imagem">Nova imagem do animal (opcional)</param>
        /// <returns>Status de sucesso ou erro</returns>
        // PUT: api/Animal/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnimal(int id, [FromForm] Animal animal, IFormFile imagem)
        {
            // Verifica se o ID corresponde
            if (id != animal.Id)
            {
                return BadRequest();
            }

            // Obtém o animal existente da base de dados
            var animalExistente = await _context.Animal.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            if (animalExistente == null)
            {
                return NotFound();
            }

            // Processa a imagem se foi fornecida
            if (imagem != null && imagem.Length > 0)
            {
                // Validações da imagem
                var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var mimeTypesValidos = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp" };
                var extensao = Path.GetExtension(imagem.FileName).ToLower();

                if (!extensoesPermitidas.Contains(extensao) || !mimeTypesValidos.Contains(imagem.ContentType.ToLower()))
                {
                    return BadRequest("O arquivo deve ser uma imagem válida.");
                }

                // Prepara o diretório para guardar a imagem
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Gera o nome do ficheiro e guarda a imagem
                var nomeArquivo = $"{animal.Id}{extensao}";
                var caminhoArquivo = Path.Combine(uploadPath, nomeArquivo);

                using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
                {
                    await imagem.CopyToAsync(stream);
                }

                // Atualiza o caminho da imagem
                animal.Imagem = "/images/" + nomeArquivo;
            }
            else
            {
                // Mantém a imagem existente se não foi fornecida nova
                animal.Imagem = animalExistente.Imagem; // mantém a imagem anterior
            }

            // Marca o objeto como modificado
            _context.Entry(animal).State = EntityState.Modified;

            try
            {
                // Tenta guardar as alterações
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Trata erros de concorrência
                if (!AnimalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
// Retorna status 204 (No Content) em caso de sucesso
            return NoContent();
        }

        /// <summary>
        /// Elimina um animal existente
        /// </summary>
        /// <param name="id">ID do animal a eliminar</param>
        /// <returns>Status de sucesso ou erro</returns>
        // DELETE: api/Animal/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnimal(int id)
        {
            // Procura o animal na base de dados
            var animal = await _context.Animal.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }

            // Processa a imagem associada ao animal
            if (!string.IsNullOrEmpty(animal.Imagem))
            {
                var nomeImagem = Path.GetFileName(animal.Imagem);
                var caminhoOriginal = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", nomeImagem);
                var pastaLixo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Lixo");

                // Cria a pasta Lixo se não existir
                if (!Directory.Exists(pastaLixo))
                {
                    Directory.CreateDirectory(pastaLixo);
                }

                // Move a imagem para a pasta Lixo
                var caminhoLixo = Path.Combine(pastaLixo, nomeImagem);
                if (System.IO.File.Exists(caminhoOriginal))
                {
                    System.IO.File.Move(caminhoOriginal, caminhoLixo, overwrite: true);
                }
            }

            // Remove o animal da base de dados
            _context.Animal.Remove(animal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Verifica se um animal com o ID especificado existe
        /// </summary>
        /// <param name="id">ID do animal</param>
        /// <returns>True se existir, False caso contrário</returns>
        private bool AnimalExists(int id)
        {
            return _context.Animal.Any(e => e.Id == id);
        }
    }
}
