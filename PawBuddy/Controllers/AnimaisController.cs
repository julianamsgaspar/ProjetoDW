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
    /// Controlador responsável pela gestão dos animais no sistema.
    /// Permite visualizar, adicionar, editar e remover animais.
    /// Também lida com o upload e validação de imagens.
    /// </summary>
    // ---------- TUDO neste controller exige Admin ----------
    [Authorize(Roles = "Admin")]
    public class AnimaisController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        /// <summary>
        /// Construtor que injeta o contexto da base de dados.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public AnimaisController(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Lista todos os animais cadastrados.
        /// </summary>
        /// <returns>Vista com a lista de animais.</returns>
        // GET: Animais
        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchNome, string especie, string genero, int page = 1, int pageSize = 6)
        {
            var animais = _context.Animal.AsQueryable();

            if (!string.IsNullOrEmpty(searchNome))
                animais = animais.Where(a => a.Nome.Contains(searchNome));

            if (!string.IsNullOrEmpty(especie))
                animais = animais.Where(a => a.Especie == especie);

            if (!string.IsNullOrEmpty(genero))
                animais = animais.Where(a => a.Genero == genero);

            int totalItems = await animais.CountAsync();

            var animaisPaginados = await animais
                .OrderBy(a => a.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewData["CurrentFilter"] = searchNome ?? "";
            ViewData["CurrentEspecie"] = especie ?? "";
            ViewData["CurrentGenero"] = genero ?? "";

            return View(animaisPaginados);
        }

        public async Task<IActionResult> IndexPartial(string searchNome, string especie, string genero)
        {
            var animais = _context.Animal.AsQueryable();

            if (!string.IsNullOrEmpty(searchNome))
                animais = animais.Where(a => a.Nome.Contains(searchNome));

            if (!string.IsNullOrEmpty(especie))
                animais = animais.Where(a => a.Especie == especie);

            if (!string.IsNullOrEmpty(genero))
                animais = animais.Where(a => a.Genero == genero);

            var listaAnimais = await animais.ToListAsync();

            return PartialView("_IndexPartial", listaAnimais);
        }

        
        /// <summary>
        /// Exibe os detalhes de um animal específico.
        /// </summary>
        /// <param name="id">ID do animal.</param>
        /// <returns>Vista de detalhes ou NotFound.</returns>
        // GET: Animais/Details/5
        
        [AllowAnonymous]  
        
        public async Task<IActionResult> Details(int? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animal
                .FirstOrDefaultAsync(m => m.Id == id);
            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }
    
        /// <summary>
        /// Exibe o formulário para criação de um novo animal.
        /// </summary>
        /// <returns>Vista de criação.</returns>
        // GET: Animais/Create
        public IActionResult Create()
        {
            return View();
        }
        
        /// <summary>
        /// Cria um novo animal e salva a imagem enviada.
        /// </summary>
        /// <param name="animal">Dados do animal.</param>
        /// <param name="imagem">Imagem enviada via formulário.</param>
        /// <returns>Redireciona ao índice ou retorna a vista com erros.</returns>
        // POST: Animais/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
           
        public async Task<IActionResult> Create([Bind("Id,Nome,Raca,Idade,Genero,Especie,Cor,Imagem")] Animal animal, IFormFile imagem)
        {
            // Evita erro de validação ao não ter uma imagem definida inicialmente
            if (string.IsNullOrEmpty(animal.Imagem))
            {
                animal.Imagem = ""; 
            }

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Erro: {error.ErrorMessage}");
                }
            }

            if (ModelState.IsValid || (imagem != null && imagem.Length > 0)) // Permitir seguir se houver imagem
            {
                try
                {
                    // Se uma imagem foi enviada
                    if (imagem != null && imagem.Length > 0)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                        var fileExtension = Path.GetExtension(imagem.FileName).ToLower();

                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("Imagem", "O arquivo deve ser uma imagem (jpg, jpeg, png, gif, bmp).");
                            return View(animal);
                        }

                        var validMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp" };
                        var mimeType = imagem.ContentType.ToLower();

                        if (!validMimeTypes.Contains(mimeType))
                        {
                            ModelState.AddModelError("Imagem", "O tipo de arquivo não é uma imagem válida.");
                            return View(animal);
                        }

                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        // Salvar temporariamente com um GUID, mas renomear após salvar no banco
                        var tempFileName = Guid.NewGuid().ToString() + fileExtension;
                        var tempFilePath = Path.Combine(uploadPath, tempFileName);

                        using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
                        {
                            await imagem.CopyToAsync(fileStream);
                        }

                        // Adiciona o animal ao banco de dados para obter um Id
                        _context.Add(animal);
                        await _context.SaveChangesAsync();

                        // Agora que temos o ID do animal, renomeamos o arquivo corretamente
                        var finalFileName = animal.Id + fileExtension;
                        var finalFilePath = Path.Combine(uploadPath, finalFileName);
                        System.IO.File.Move(tempFilePath, finalFilePath);

                        // Atualizar caminho da imagem no modelo e salvar novamente
                        animal.Imagem = "/images/" + finalFileName;
                        _context.Update(animal);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ModelState.AddModelError("Imagem", "A imagem é obrigatória.");
                        return View(animal);
                    }

                    Console.WriteLine($"Animal criado com ID: {animal.Id}");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao criar animal: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "Ocorreu um erro ao tentar criar o animal.");
                }
            }

            return View(animal);
        }

        /// <summary>
        /// Exibe o formulário para editar um animal existente.
        /// </summary>
        /// <param name="id">ID do animal.</param>
        /// <returns>Vista de edição ou NotFound.</returns>
        // GET: Animais/Edit/5
       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animal.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            return View(animal);
        }
        
        /// <summary>
        /// Atualiza os dados do animal e substitui a imagem, se uma nova for enviada.
        /// </summary>
        /// <param name="id">ID do animal a ser editado.</param>
        /// <param name="animal">Objeto com os dados atualizados.</param>
        /// <param name="imagem">Nova imagem, se houver.</param>
        /// <returns>Redireciona ao índice ou retorna a vista com erros.</returns>
        // POST: Animais/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Raca,Idade,Genero,Especie,Cor,Imagem")] Animal animal, IFormFile imagem)
        {
            if (id != animal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Buscar o animal original do banco de dados
                    var animalExistente = await _context.Animal.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
                    if (animalExistente == null)
                    {
                        return NotFound();
                    }

                    // Se nenhuma nova imagem foi enviada, mantém a imagem existente
                    if (imagem == null || imagem.Length == 0)
                    {
                        animal.Imagem = animalExistente.Imagem;
                    }
                    else
                    {
                        // Verifique a extensão do arquivo (apenas imagens)
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                        var fileExtension = Path.GetExtension(imagem.FileName).ToLower();

                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("Imagem", "O arquivo deve ser uma imagem (jpg, jpeg, png, gif, bmp).");
                            return View(animal);
                        }

                        // Verifique o tipo MIME do arquivo
                        var validMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp" };
                        var mimeType = imagem.ContentType.ToLower();

                        if (!validMimeTypes.Contains(mimeType))
                        {
                            ModelState.AddModelError("Imagem", "O tipo de arquivo não é uma imagem válida.");
                            return View(animal);
                        }

                        // Defina o diretório de upload e crie o diretório se não existir
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        // Gere um nome único para a imagem
                        var fileName = Guid.NewGuid().ToString() + fileExtension;
                        var filePath = Path.Combine(uploadPath, fileName);

                        // Salve o arquivo no servidor
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imagem.CopyToAsync(fileStream);
                        }

                        // Atualize o caminho da imagem no modelo
                        animal.Imagem = "/images/" + fileName;
                    }

                    // Atualize o animal no banco de dados
                    _context.Update(animal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnimalExists(animal.Id))
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

            return View(animal);
        }

        /// <summary>
        /// Exibe a confirmação para remover um animal.
        /// </summary>
        /// <param name="id">ID do animal.</param>
        /// <returns>Vista de confirmação ou NotFound.</returns>
        // GET: Animais/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animal
                .FirstOrDefaultAsync(m => m.Id == id);
            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }
        
        /// <summary>
        /// Confirma e executa a exclusão lógica de um animal do sistema.
        /// Em vez de apagar permanentemente a imagem do animal, ela é movida para a pasta "Lixo"
        /// dentro de "wwwroot/images" para possibilitar futura restauração ou auditoria.
        /// </summary>
        /// <param name="id">O identificador único (ID) do animal a ser excluído.</param>
        /// <returns>Redireciona para a view <c>Index</c> após a exclusão ser concluída.</returns>
        /// <remarks>
        /// Se o animal tiver uma imagem associada, o arquivo será movido para o diretório
        /// <c>wwwroot/images/Lixo</c>. Se o diretório não existir, ele será criado automaticamente.
        /// </remarks>
        // POST: Animais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var animal = await _context.Animal.FindAsync(id);
            if (animal != null)
            {
                // Verifica se há imagem associada
                if (!string.IsNullOrEmpty(animal.Imagem))
                {
                    var imagemNome = Path.GetFileName(animal.Imagem); // extrai "123.jpg"
                    var imagemPathOriginal = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", imagemNome);
                    var pastaLixo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Lixo");

                    // Cria diretório Lixo se não existir
                    if (!Directory.Exists(pastaLixo))
                    {
                        Directory.CreateDirectory(pastaLixo);
                    }

                    var imagemPathDestino = Path.Combine(pastaLixo, imagemNome);

                    // Move a imagem para o diretório "Lixo" se existir
                    if (System.IO.File.Exists(imagemPathOriginal))
                    {
                        System.IO.File.Move(imagemPathOriginal, imagemPathDestino, overwrite: true);
                    }
                }

                _context.Animal.Remove(animal);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        
        // <summary>
        /// Verifica se um animal existe na base de dados.
        /// </summary>
        /// <param name="id">ID do animal.</param>
        /// <returns>True se existir, False caso contrário.</returns>
        private bool AnimalExists(int id)
        {
            return _context.Animal.Any(e => e.Id == id);
        }
    }
}
