using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PawBuddy.Models.ApiModels;

namespace PawBuddy.Controllers.API
{
    /// <summary>
    /// Controlador API para autenticação de utilizadores
    /// Responsável por operações de login, logout e verificação de autenticação
    /// </summary>
    [Route("api/AuthController")] // Define a rota base para os endpoints
    [ApiController] // Indica que é um controlador API
    public class AuthController : ControllerBase
    {   // Gestor de autenticação (SignInManager)
        private readonly SignInManager<IdentityUser> _signInManager;
        // Gestor de utilizadores (UserManager)
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Construtor que recebe os serviços de autenticação por injeção de dependência
        /// </summary>
        /// <param name="signInManager">Serviço para gestão de autenticação</param>
        /// <param name="userManager">Serviço para gestão de utilizadores</param>
        public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Endpoint de teste para verificar autenticação
        /// Requer que o utilizador esteja autenticado
        /// </summary>
        /// <returns>Mensagem de saudação com o nome do utilizador</returns>
        [HttpGet("hello")]
        [Authorize] 
        public ActionResult Hello()
        {
            // Retorna mensagem personalizada com o nome do utilizador autenticado
            return Ok($"Hello, {User.Identity.Name}!");
        }

        /// <summary>
        /// Endpoint para autenticação de utilizadores
        /// </summary>
        /// <param name="loginRequest">Objeto com credenciais de login (email e password)</param>
        /// <returns>Resultado da operação de login</returns>
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginModel loginRequest)
        {
            // Tenta autenticar o utilizador com as credenciais fornecidas
            var result = await _signInManager.PasswordSignInAsync(
                loginRequest.Email,   // Email do utilizador
                loginRequest.Password,          // Password do utilizador
                isPersistent: false,            // Não manter a sessão após fecho do browser
                lockoutOnFailure: false         // Não bloquear conta após falhas
            );

            // Se a autenticação foi bem sucedida
            if (result.Succeeded)
            {
                // Retorna mensagem de sucesso e email do utilizador
                return Ok(new { message = "Login successful", user = loginRequest.Email });
            }

            // Se a autenticação falhou
            return BadRequest("Erro no login. Verifique o utilizador e a password.");
        }

        /// <summary>
        /// Endpoint para terminar a sessão do utilizador
        /// </summary>
        /// <returns>Status 204 (NoContent) em caso de sucesso</returns>
        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            // Termina a sessão do utilizador
            await _signInManager.SignOutAsync();
            // Retorna status 204 (No Content) indicando sucesso sem conteúdo para retornar
            return NoContent();
        }
    }
}