// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace PawBuddy.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _sender;

        public RegisterConfirmationModel(UserManager<IdentityUser> userManager, IEmailSender sender)
        {
            _userManager = userManager;
            _sender = sender;
        }

        /// <summary>
        ///     Email do utilizador que será confirmado.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     Flag para determinar se o link de confirmação de conta será mostrado.
        /// </summary>
        public bool DisplayConfirmAccountLink { get; set; }

        /// <summary>
        ///     URL para a página de confirmação de email.
        /// </summary>
        public string EmailConfirmationUrl { get; set; }

        /// <summary>
        ///     Método para gerar a página de confirmação de registro do utilizador.
        /// </summary>
        /// <param name="email">Email do utilizador.</param>
        /// <param name="returnUrl">URL para redirecionar após confirmação.</param>
        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            // Se o email não for fornecido, redireciona para a página principal
            if (email == null)
            {
                return RedirectToPage("/Index");
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Carregar o utilizador pelo email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Se o utilizador não for encontrado, retorna erro
                return NotFound($"Não foi possível carregar o utilizador com o email '{email}'.");
            }

            // Definir o email para exibição
            Email = email;

            // Aqui, é onde você configura se deseja exibir o link de confirmação
            DisplayConfirmAccountLink = true; // Definido como true para exibir link sempre

            if (DisplayConfirmAccountLink)
            {
                // Gerar o token de confirmação de email para o utilizador
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // Codificar o token de confirmação para garantir que é seguro na URL
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                // Gerar a URL de confirmação com o token
                EmailConfirmationUrl = Url.Page(
                    "/Account/ConfirmEmail",  // A página que processa a confirmação do email
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);
            }

            // Retornar a página com os dados gerados
            return Page();
        }
    }
}
