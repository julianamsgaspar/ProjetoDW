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

        // GET: Administrador/ListaAdmin
        [HttpGet("ListaAdmin")]
        public async Task<IActionResult> ListaAdmin()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Administrador");
            return View(admins);
        }

        // GET: Administrador/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

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

        // POST: Administrador/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Nome, Idade,Nif,Telemovel,Morada,CodPostal,Email,Pais")] Utilizador utilizador)
        {
            if (id != utilizador.IdentityUserId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Atualizar Utilizador
                    _context.Update(utilizador);
                    await _context.SaveChangesAsync();

                    // Atualizar IdentityUser
                    var identityUser = await _userManager.FindByIdAsync(id);
                    if (identityUser != null)
                    {
                        identityUser.UserName = utilizador.Email;
                        identityUser.Email = utilizador.Email;
                        identityUser.PhoneNumber = utilizador.Telemovel;
                        await _userManager.UpdateAsync(identityUser);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilizadorExists(utilizador.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(ListaAdmin));
            }
            return View(utilizador);
        }

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

        private bool UtilizadorExists(int id)
        {
            return _context.Utilizador.Any(e => e.Id == id);
        }
    }
}