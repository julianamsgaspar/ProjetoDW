using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawBuddy.Data;
using PawBuddy.Models;

namespace PawBuddy.Controllers
{
    [Route("Administrador")]
    public class AdministradorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        public AdministradorController(
            UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        
        /// <summary>
        /// Lista todos os utilizadores com a role "Administrador".
        /// </summary>
        /// <returns>Vista com a lista de administradores.</returns>
        // GET: Administrador/ListaAdmin
        [HttpGet("ListaAdmin")]
        public async Task<IActionResult> ListaAdmin()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Administrador");
            return View(admins);
        }
        
        /// <summary>
        /// Exibe o formulário para criação de um novo administrador.
        /// </summary>
        /// <returns>Vista de criação.</returns>
        // GET: Administrador/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }
        
        /// <summary>
        /// Processa os dados do formulário e cria um novo administrador.
        /// </summary>
        /// <param name="Nome">Nome do utilizador.</param>
        /// <param name="Telemovel">Número de telefone.</param>
        /// <param name="Email">Email do utilizador.</param>
        /// <param name="Nif">Número de identificação fiscal.</param>
        /// <param name="Morada">Endereço do utilizador.</param>
        /// <param name="CodPostal">Código postal.</param>
        /// <param name="Pais">País de residência.</param>
        /// <param name="Idade">Data de nascimento.</param>
        /// <param name="Password">Palavra-passe inicial.</param>
        /// <returns>Redireciona para a lista se bem-sucedido ou retorna vista com erros.</returns>
        // POST: Administrador/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Nome, string Telemovel, string Email, string Nif, string Morada, string CodPostal, string Pais, DateTime Idade,  string Password)
        {
            var identityUser = new IdentityUser
            {
                UserName = Nome, 
                Email = Email,
                PhoneNumber = Telemovel,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, Password);
                
            if (result.Succeeded)
            {
                // Criar role se não existir
                if (!await _roleManager.RoleExistsAsync("Administrador"))
                    await _roleManager.CreateAsync(new IdentityRole("Administrador"));

                await _userManager.AddToRoleAsync(identityUser, "Administrador");

                // Criar registro na tabela Utilizador
                var novoUtilizador = new Utilizador
                {
                    Nome = Nome,
                    Telemovel = Telemovel,
                    Email = Email,
                    IdentityUserId = identityUser.Id, // Importante: guardar a referência
                    Morada = Morada,
                    Nif = Nif,
                    CodPostal = CodPostal,
                    Pais = Pais,
                    DataNascimento = Idade
                };

                _context.Utilizador.Add(novoUtilizador);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ListaAdmin));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            
            return View();
        }
        
        /// <summary>
        /// Obtém os dados do administrador para edição.
        /// </summary>
        /// <param name="id">ID do IdentityUser.</param>
        /// <returns>Vista de edição.</returns>
        // GET: Administrador/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var identityUser = await _userManager.FindByIdAsync(id);
            if (identityUser == null)
                return NotFound();

            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(u => u.IdentityUserId == id);

            ViewData["IdentityUser"] = identityUser;
            return View(utilizador);
        }
        
        /// <summary>
        /// Atualiza os dados do administrador tanto no Identity como na base de dados da aplicação.
        /// </summary>
        /// <param name="id">ID do IdentityUser.</param>
        /// <param name="Nome">Nome atualizado.</param>
        /// <param name="Telemovel">Telefone atualizado.</param>
        /// <param name="Email">Email atualizado.</param>
        /// <param name="Nif">NIF atualizado.</param>
        /// <param name="Morada">Morada atualizada.</param>
        /// <param name="CodPostal">Código postal atualizado.</param>
        /// <param name="Pais">País atualizado.</param>
        /// <param name="Password">Nova password (opcional).</param>
        /// <param name="Id">ID da tabela Utilizador.</param>
        /// <param name="IdentityUserId">ID do IdentityUser (validação cruzada).</param>
        /// <returns>Redireciona após atualização.</returns>
        // POST: Administrador/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, 
            string Nome, 
            string Telemovel, 
            string Email, 
            string Nif,
            string Morada,
            string CodPostal,
            string Pais,
            string Password,
            int Id, // From hidden field
            string IdentityUserId) // From hidden field
        {
            if (id != IdentityUserId)
                return NotFound();

            // Update Utilizador
            var utilizador = await _context.Utilizador.FindAsync(Id);
            if (utilizador == null)
                return NotFound();

            utilizador.Nome = Nome;
            utilizador.Telemovel = Telemovel;
            utilizador.Email = Email;
            utilizador.Nif = Nif;
            utilizador.Morada = Morada;
            utilizador.CodPostal = CodPostal;
            utilizador.Pais = Pais;

            _context.Update(utilizador);
            await _context.SaveChangesAsync();

            // Update IdentityUser
            var identityUser = await _userManager.FindByIdAsync(id);
            if (identityUser != null)
            {
                identityUser.UserName = Email;
                identityUser.Email = Email;
                identityUser.PhoneNumber = Telemovel;
        
                if (!string.IsNullOrEmpty(Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
                    await _userManager.ResetPasswordAsync(identityUser, token, Password);
                }
        
                await _userManager.UpdateAsync(identityUser);
            }

            return RedirectToAction(nameof(ListaAdmin));
        }
        
        /// <summary>
        /// Exibe os detalhes de um administrador específico.
        /// </summary>
        /// <param name="id">ID do IdentityUser.</param>
        /// <returns>Vista de detalhes do administrador.</returns>
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var identityUser = await _userManager.FindByIdAsync(id);
            if (identityUser == null)
                return NotFound();

            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(u => u.IdentityUserId == id);

            if (utilizador == null)
                return NotFound();

            // Create a view model or use ViewData to pass both objects
            ViewData["IdentityUser"] = identityUser;
            return View(utilizador); // Or create a ViewModel that combines both
        }
        
        /// <summary>
        /// Exibe a confirmação de eliminação de um administrador.
        /// </summary>
        /// <param name="id">ID do IdentityUser.</param>
        /// <returns>Vista de confirmação.</returns>
        // GET: Administrador/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var identityUser = await _userManager.FindByIdAsync(id);
            if (identityUser == null)
                return NotFound();

            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(u => u.IdentityUserId == id);

            if (utilizador == null)
            {
                // If Utilizador not found, we can still delete just the IdentityUser
                ViewData["Utilizador"] = new Utilizador(); // Empty object to prevent null reference
            }
            else
            {
                ViewData["Utilizador"] = utilizador;
            }

            return View(identityUser); // Main model is IdentityUser
        }
        
        /// <summary>
        /// Elimina permanentemente um administrador (Identity e dados associados).
        /// </summary>
        /// <param name="id">ID do utilizador a eliminar.</param>
        /// <returns>Redireciona para a lista após eliminação.</returns>
        // POST: Administrador/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var identityUser = await _userManager.FindByIdAsync(id);
            if (identityUser == null)
                return NotFound();

            // Verificar se é o próprio usuário
            if (_userManager.GetUserId(User) == id)
            {
                TempData["Erro"] = "Não pode eliminar sua própria conta!";
                return RedirectToAction(nameof(ListaAdmin));
            }

            // Remover da tabela Utilizador
            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(u => u.IdentityUserId == id);
            
            if (utilizador != null)
            {
                _context.Utilizador.Remove(utilizador);
                await _context.SaveChangesAsync();
            }

            // Remover do Identity
            var result = await _userManager.DeleteAsync(identityUser);
            if (!result.Succeeded)
            {
                TempData["Erro"] = "Erro ao eliminar o administrador.";
                return RedirectToAction(nameof(ListaAdmin));
            }

            TempData["Sucesso"] = "Administrador eliminado com sucesso!";
            return RedirectToAction(nameof(ListaAdmin));
        }

        /// <summary>
        /// Verifica se um utilizador existe na base de dados da aplicação.
        /// </summary>
        /// <param name="id">ID da tabela Utilizador.</param>
        /// <returns>True se existir, caso contrário False.</returns>
        private bool UtilizadorExists(int id)
        {
            return _context.Utilizador.Any(e => e.Id == id);
        }
    }
}