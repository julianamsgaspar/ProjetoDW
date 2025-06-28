using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawBuddy.Data;
using PawBuddy.Models;
using Microsoft.AspNetCore.Authorization;
using PawBuddy.ViewModels;

namespace PawBuddy.Controllers
{
    // Autoriza apenas usuários com papel "Admin" a acessar essa controller
    [Authorize(Roles = "Admin")] 
    // [Route("Administrador")] // Rota customizada (comentada)
    public class AdministradorController : Controller
    {
        // Dependências injetadas: contexto do EF, gerenciadores de usuário e de papel (roles)
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // Construtor recebe e armazena as dependências
        public AdministradorController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: Administrador/Index
        [HttpGet]
        public IActionResult Index() => View();

        /// <summary>
        /// Lista todos os utilizadores com a role "Administrador".
        /// </summary>
        public async Task<IActionResult> ListaAdmin()
        {
            // Pega todos usuários no papel "Admin"
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            return View(admins);
        }

        // Retorna uma partial view com a lista de administradores (para AJAX)
        public async Task<IActionResult> ListaAdminPartial()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            return PartialView("_ListaAdminPartial", admins);
        }

        /// <summary>
        /// Exibe o formulário para criação de um novo administrador.
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// Processa o formulário para criar um novo administrador.
        /// </summary>
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Nome, string Telemovel, string Email, string Nif, string Morada,
            string CodPostal, string Pais, DateTime Idade, string Password)
        {
            // Cria objeto IdentityUser para autenticação
            var identityUser = new IdentityUser
            {
                UserName = Nome,
                Email = Email,
                PhoneNumber = Telemovel,
                EmailConfirmed = true
            };

            // Cria o usuário no Identity com senha fornecida
            var result = await _userManager.CreateAsync(identityUser, Password);

            if (result.Succeeded)
            {
                // Garante que o papel "Admin" existe, senão cria
                if (!await _roleManager.RoleExistsAsync("Admin"))
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));

                // Adiciona o usuário ao papel "Admin"
                await _userManager.AddToRoleAsync(identityUser, "Admin");

                // Cria um objeto Utilizador para guardar dados na tabela personalizada
                var novoUtilizador = new Utilizador
                {
                    Nome = Nome,
                    Telemovel = Telemovel,
                    Email = Email,
                    IdentityUserId = identityUser.Id,
                    Morada = Morada,
                    Nif = Nif,
                    CodPostal = CodPostal,
                    Pais = Pais,
                    DataNascimento = Idade
                };

                // Adiciona e salva no banco de dados
                _context.Utilizador.Add(novoUtilizador);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index"); // ou "ListaAdmin", conforme tiveres


                // Se quiser desativar redirecionamento, pode comentar a linha abaixo:
                // return RedirectToAction(nameof(ListaAdmin));
            }

            // Se falhar criação, adiciona erros ao ModelState para exibir na view
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            // Retorna a view com os dados preenchidos para correção
            return View(new Utilizador
            {
                Nome = Nome,
                Telemovel = Telemovel,
                Email = Email,
                Nif = Nif,
                Morada = Morada,
                CodPostal = CodPostal,
                Pais = Pais,
                DataNascimento = Idade
            });
        }
       
        /// <summary>
        /// Exibe formulário de edição de administrador.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
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

            ViewData["IdentityUser"] = identityUser;

            return View(utilizador); // Modelo principal é Utilizador
        }


        /// <summary>
        /// Atualiza os dados do administrador no Identity e na tabela personalizada.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, int utilizadorId, string Nome, DateTime DataNascimento, string Telemovel, string Email, string Nif, string Morada, string CodPostal, string Pais, string Password)
        {
            // Busca o utilizador na tabela personalizada
            var utilizador = await _context.Utilizador.FindAsync(utilizadorId);
            if (utilizador == null) return NotFound();

            // Atualiza dados do utilizador
            utilizador.Nome = Nome;
            utilizador.DataNascimento = DataNascimento;
            utilizador.Telemovel = Telemovel;
            utilizador.Email = Email;
            utilizador.Nif = Nif;
            utilizador.Morada = Morada;
            utilizador.CodPostal = CodPostal;
            utilizador.Pais = Pais;

            _context.Update(utilizador);
            await _context.SaveChangesAsync();

            // Atualiza o usuário no Identity
            var identityUser = await _userManager.FindByIdAsync(id);
            if (identityUser != null)
            {
                identityUser.UserName = Nome;
                identityUser.Email = Email;
                identityUser.PhoneNumber = Telemovel;

                // Se foi fornecida nova senha, atualiza
                if (!string.IsNullOrEmpty(Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
                    await _userManager.ResetPasswordAsync(identityUser, token, Password);
                }

                await _userManager.UpdateAsync(identityUser);
            }

            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// Exibe detalhes do administrador.
        /// </summary>
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

            var model = new AdminViewModel
            {
                IdentityUser = identityUser,
                Utilizador = utilizador
            };

            return View(model);
        }

        /// <summary>
        /// Exibe confirmação de eliminação de administrador.
        /// </summary>
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
                // Para evitar erro de referência nula, cria objeto vazio
                ViewData["Utilizador"] = new Utilizador();
            }
            else
            {
                ViewData["Utilizador"] = utilizador;
            }

            return View(identityUser); // Modelo principal é IdentityUser
        }

        
        /// <summary>
        /// Remove permanentemente um administrador.
        /// </summary>
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var identityUser = await _userManager.FindByIdAsync(id);
            if (identityUser == null)
                return NotFound();

            // Previne autoexclusão
            if (_userManager.GetUserId(User) == id)
            {
                TempData["Erro"] = "Não pode eliminar sua própria conta!";
                return RedirectToAction(nameof(Index));
            }

            // Remove da tabela personalizada
            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(u => u.IdentityUserId == id);
            
            if (utilizador != null)
            {
                _context.Utilizador.Remove(utilizador);
                await _context.SaveChangesAsync();
            }

            // Remove do Identity
            var result = await _userManager.DeleteAsync(identityUser);
            if (!result.Succeeded)
            {
                TempData["Erro"] = "Erro ao eliminar o administrador.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Sucesso"] = "Administrador eliminado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        
        /// <summary>
        /// Verifica se um utilizador existe no banco de dados da aplicação.
        /// </summary>
        private bool UtilizadorExists(int id)
        {
            return _context.Utilizador.Any(e => e.Id == id);
        }

    }
}
