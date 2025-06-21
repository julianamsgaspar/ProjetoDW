using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawBuddy.Data;
using PawBuddy.Models;

namespace PawBuddy.Controllers
{
    [Route("Administrador")]  // Rota base para todas as ações neste controller
    public class AdministradorController : Controller
    {
        // Contexto da base de dados para dados específicos da aplicação
        private readonly ApplicationDbContext _context;
        // Gestor de utilizadores para operações com IdentityUser
        private readonly UserManager<IdentityUser> _userManager;
        // Gestor de roles para operações com IdentityRole
        private readonly RoleManager<IdentityRole> _roleManager;
        
        // Construtor com injeção de dependências
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
            // Obter todos os utilizadores com a role "Admin"
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            return View(admins); // Retornar vista com a lista
        }
        
        /// <summary>
        /// Exibe o formulário para criação de um novo administrador.
        /// </summary>
        /// <returns>Vista de criação.</returns>
        // GET: Administrador/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View(); // Retornar vista vazia para criação
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
            // Criar novo objeto IdentityUser com os dados recebidos
            var identityUser = new IdentityUser
            {
                UserName = Nome, 
                Email = Email,
                PhoneNumber = Telemovel,
                EmailConfirmed = true      // Confirmar email automaticamente
            };
            // Tentar criar o utilizador no sistema de autenticação
            var result = await _userManager.CreateAsync(identityUser, Password);
                
            if (result.Succeeded)   // Se o utilizador foi criado com sucesso
            {
                // Criar role "Admin" se não existir
                if (!await _roleManager.RoleExistsAsync("Admin"))
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                // Adicionar o utilizador à role "Admin"
                await _userManager.AddToRoleAsync(identityUser, "Admin");

                // Criar registo na tabela Utilizador
                var novoUtilizador = new Utilizador
                {
                    Nome = Nome,
                    Telemovel = Telemovel,
                    Email = Email,
                    IdentityUserId = identityUser.Id, // Guardar a referência ao IdentityUser
                    Morada = Morada,
                    Nif = Nif,
                    CodPostal = CodPostal,
                    Pais = Pais,
                    DataNascimento = Idade
                };
                // Adicionar e guardar na base de dados
                _context.Utilizador.Add(novoUtilizador);
                await _context.SaveChangesAsync();
                // Redirecionar para a lista de administradores
                return RedirectToAction(nameof(ListaAdmin));
            }
            // Se houve erros, adicioná-los ao ModelState
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            // Voltar a mostrar a vista com os erros 
            return View();
        }
        
        /// <summary>
        /// Exibe a vista de edição de um administrador
        /// </summary>
        /// <param name="id">ID do IdentityUser</param>
        /// <returns>Vista de edição</returns>
        [HttpGet("Edit/{Id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))   // Verificar se o ID é válido
                return NotFound();
            // Procurar o utilizador no sistema de autenticação
            var identityUser = await _userManager.FindByIdAsync(id);
            if (identityUser == null)
                return NotFound();
            // Procurar o utilizador na base de dados da aplicação
            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(u => u.IdentityUserId == id);

            if (utilizador == null)
                return NotFound();
            // Passar o IdentityUser para a vista através de ViewData
            ViewData["IdentityUser"] = identityUser;
            // Retornar vista com o modelo principal (Utilizador)
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
        public async Task<IActionResult> Edit(
            string identityUserId,     // ID do IdentityUser vindo da rota
            int utilizadorId,          // ID do Utilizador vindo do formulário
            string Nome,
            DateTime DataNascimento,
            string Telemovel,
            string Email,
            string Nif,
            string Morada,
            string CodPostal,
            string Pais,
            string Password
        )
        {
            // Procurar o utilizador na base de dados
            var utilizador = await _context.Utilizador.FindAsync(utilizadorId); // Id é int
            if (utilizador == null)
            {
                return NotFound();
            }
                
            // Atualizar os dados do utilizador
            utilizador.Nome = Nome;
            utilizador.DataNascimento = DataNascimento;
            utilizador.Telemovel = Telemovel;
            utilizador.Email = Email;
            utilizador.Nif = Nif;
            utilizador.Morada = Morada;
            utilizador.CodPostal = CodPostal;
            utilizador.Pais = Pais;

            // Guardar alterações na base de dados
            _context.Update(utilizador);
            await _context.SaveChangesAsync();

            // Atualizar o IdentityUser
            var identityUser = await _userManager.FindByIdAsync(identityUserId);
            if (identityUser != null)
            {
                // Atualizar propriedades básicas
                identityUser.UserName = Nome;
                identityUser.Email = Email;
                identityUser.PhoneNumber = Telemovel;

                // Se foi fornecida uma nova palavra-passe, atualizá-la
                if (!string.IsNullOrEmpty(Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
                    await _userManager.ResetPasswordAsync(identityUser, token, Password);
                }

                // Guardar alterações no Identity
                await _userManager.UpdateAsync(identityUser);
            }

            // Redirecionar para a lista de administradores
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
            if (string.IsNullOrEmpty(id))  // Verificar se o ID é válido
                return NotFound();

            // Procurar o utilizador no sistema de autenticação
            var identityUser = await _userManager.FindByIdAsync(id);
            if (identityUser == null)
                return NotFound();
            
            // Procurar o utilizador na base de dados
            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(u => u.IdentityUserId == id);

            if (utilizador == null)
                return NotFound();

            // Passar o IdentityUser para a vista através de ViewData
            ViewData["IdentityUser"] = identityUser;
            return View(utilizador); // Modelo principal é o Utilizador
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
            if (string.IsNullOrEmpty(id))// Verificar se o ID é válido
                return NotFound();
            // Procurar o utilizador no sistema de autenticação
            var identityUser = await _userManager.FindByIdAsync(id);
            if (identityUser == null)
                return NotFound();

            // Procurar o utilizador na base de dados
            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(u => u.IdentityUserId == id);

            if (utilizador == null)
            {
                // Se não encontrar na base de dados, criar objeto vazio
                ViewData["Utilizador"] = new Utilizador(); 
            }
            else
            {
                ViewData["Utilizador"] = utilizador;
            }

            return View(identityUser); // Modelo principal é o IdentityUser
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
            // Procurar o utilizador no sistema de autenticação
            var identityUser = await _userManager.FindByIdAsync(id);
            if (identityUser == null)
                return NotFound();

            // Verificar se é o próprio utilizador a tentar eliminar-se
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

            // Remover do sistema de autenticação
            var result = await _userManager.DeleteAsync(identityUser);
            if (!result.Succeeded)
            {
                TempData["Erro"] = "Erro ao eliminar o administrador.";
                return RedirectToAction(nameof(ListaAdmin));
            }
            // Mensagem de sucesso e redirecionamento
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