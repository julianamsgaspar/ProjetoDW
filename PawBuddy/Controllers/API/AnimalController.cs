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
    [Route("api/Animal")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AnimalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Animal
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAnimal()
        {
            return await _context.Animal.ToListAsync();
        }

        // GET: api/Animal/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Animal>> GetAnimal(int id)
        {
            var animal = await _context.Animal.FindAsync(id);

            if (animal == null)
            {
                return NotFound();
            }

            return animal;
        }

        // POST: api/Animal
        [HttpPost]
        public async Task<ActionResult<Animal>> PostAnimal(  [FromForm] Animal animal, IFormFile imagem)
        {
            if (imagem == null || imagem.Length == 0)
            {
                return BadRequest("Imagem é obrigatória.");
            }

            var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var mimeTypesValidos = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp" };
            var extensao = Path.GetExtension(imagem.FileName).ToLower();

            if (!extensoesPermitidas.Contains(extensao) || !mimeTypesValidos.Contains(imagem.ContentType.ToLower()))
            {
                return BadRequest("O arquivo deve ser uma imagem válida.");
            }

            // Salva o animal para gerar o ID
            _context.Animal.Add(animal);
            await _context.SaveChangesAsync();

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var nomeArquivo = $"{animal.Id}{extensao}";
            var caminhoArquivo = Path.Combine(uploadPath, nomeArquivo);

            using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
            {
                await imagem.CopyToAsync(stream);
            }

            animal.Imagem = "/images/" + nomeArquivo;
            _context.Update(animal);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAnimal), new { id = animal.Id }, animal);
        }

        // PUT: api/Animal/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnimal(int id, [FromForm] Animal animal, IFormFile imagem)
        {
            if (id != animal.Id)
            {
                return BadRequest();
            }

            var animalExistente = await _context.Animal.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            if (animalExistente == null)
            {
                return NotFound();
            }

            if (imagem != null && imagem.Length > 0)
            {
                var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var mimeTypesValidos = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp" };
                var extensao = Path.GetExtension(imagem.FileName).ToLower();

                if (!extensoesPermitidas.Contains(extensao) || !mimeTypesValidos.Contains(imagem.ContentType.ToLower()))
                {
                    return BadRequest("O arquivo deve ser uma imagem válida.");
                }

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var nomeArquivo = $"{animal.Id}{extensao}";
                var caminhoArquivo = Path.Combine(uploadPath, nomeArquivo);

                using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
                {
                    await imagem.CopyToAsync(stream);
                }

                animal.Imagem = "/images/" + nomeArquivo;
            }
            else
            {
                animal.Imagem = animalExistente.Imagem; // mantém a imagem anterior
            }

            _context.Entry(animal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Animal/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnimal(int id)
        {
            var animal = await _context.Animal.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(animal.Imagem))
            {
                var nomeImagem = Path.GetFileName(animal.Imagem);
                var caminhoOriginal = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", nomeImagem);
                var pastaLixo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Lixo");

                if (!Directory.Exists(pastaLixo))
                {
                    Directory.CreateDirectory(pastaLixo);
                }

                var caminhoLixo = Path.Combine(pastaLixo, nomeImagem);
                if (System.IO.File.Exists(caminhoOriginal))
                {
                    System.IO.File.Move(caminhoOriginal, caminhoLixo, overwrite: true);
                }
            }

            _context.Animal.Remove(animal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AnimalExists(int id)
        {
            return _context.Animal.Any(e => e.Id == id);
        }
    }
}
