// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace PawBuddy.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Classe responsável pelo processo de login
    /// </summary>
    public class LoginModel : PageModel
    {
        // Injeta o serviço de autenticação e de logging
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        /// <summary>
        /// Construtor: inicializa os serviços injetados
        /// </summary>
        /// <param name="signInManager"></param>
        /// <param name="logger"></param>
        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "O campo {0} é obrigatório.")]
            [Display(Name = "Username ou Email")]
            public string UsernameOrEmail { get; set; }
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "O campo {0} é obrigatório.")]
            [Display(Name = "Password")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Lembrar-me?")]
            public bool RememberMe { get; set; }
            
        }


        /// <summary>
        /// Método que é executado quando a página é acedida via GET
        /// </summary>
        /// <param name="returnUrl"></param>
        public async Task OnGetAsync(string returnUrl = null)
        {
            // Se houver uma mensagem de erro anterior, adiciona-a ao modelo
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            // Define o URL de retorno por omissão
            returnUrl ??= Url.Content("~/");

            // Limpa cookies externos anteriores (caso tenha havido login via terceiros)
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Obtém os esquemas de autenticação externa disponíveis
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        /// <summary>
        ///   // Método que é executado quando o formulário de login é submetido (POST)
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/"); // URL de retorno por omissão

            // Obtém esquemas externos novamente (em caso de erro ou reexibição do formulário)
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Verifica se os dados submetidos são válidos
            if (ModelState.IsValid)
            {
                string userName = Input.UsernameOrEmail;

                // Se o utilizador forneceu um email, tenta encontrar o nome de utilizador correspondente
                if (Input.UsernameOrEmail.Contains("@"))
                {
                    // Procurar usuário pelo email
                    var user = await _signInManager.UserManager.FindByEmailAsync(Input.UsernameOrEmail);
                    if (user != null)
                    {
                        userName = user.UserName;
                    }
                }

                // Tenta autenticar o utilizador com nome de utilizador, palavra-passe e opção "lembrar-me"
                var result = await _signInManager.PasswordSignInAsync(userName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    // Log de sucesso
                    _logger.LogInformation("Utilizador registado com sucesso.");
                    
                    
                    // Guardar o ID do utilizador autenticado na sessão
                    //var user = await _signInManager.UserManager.FindByNameAsync(userName);

                    //if (user != null) HttpContext.Session.SetString("UserId", user.Id);

                    // Redireciona para a página de retorno
                    return LocalRedirect(returnUrl);
                    
                    
                }
                // Caso o utilizador tenha 2FA ativado
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                // Caso a conta esteja bloqueada
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Conta Bloqueada."); // Log de conta bloqueada
                    return RedirectToPage("./Lockout");             // Redireciona para página de bloqueio
                }
                else
                {
                    // Caso o login seja inválido (credenciais erradas)
                    ModelState.AddModelError(string.Empty, "Tentativa de acesso ao sistema inválida.");
                    return Page();// Reapresenta a página de login com erro
                }
            }

            // Se o modelo não for válido, apresenta novamente o formulário
            return Page();
        }

    }
}
