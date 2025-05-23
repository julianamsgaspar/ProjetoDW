using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using PawBuddy.Data;
using PawBuddy.Models;

namespace PawBuddy.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            /*[Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }*/

            [Required]
            [StringLength(100, ErrorMessage = "A palavra-passe deve ter entre {2} e {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Password")]
            [Compare("Password", ErrorMessage = "A password e a confirmação não coincidem.")]
            public string ConfirmPassword { get; set; }

            public Utilizador Utilizador { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
{
    _logger.LogInformation("Iniciando processo de registro..."); // <-- NOVO
    
    if (!ModelState.IsValid)
    {
        _logger.LogWarning("ModelState inválido. Erros: " + 
                           string.Join(", ", ModelState.Values
                               .SelectMany(v => v.Errors)
                               .Select(e => e.ErrorMessage))); // <-- NOVO
        return Page();
    }
    returnUrl ??= Url.Content("~/");
    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

    if (!ModelState.IsValid) return Page();

    await using var transaction = await _context.Database.BeginTransactionAsync();
    
    try
    {
        var user = CreateUser();
        await _userStore.SetUserNameAsync(user, Input.Utilizador.Email, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, Input.Utilizador.Email, CancellationToken.None);

        var result = await _userManager.CreateAsync(user, Input.Password);

        if (result.Succeeded)
        {
            // Preenche e guarda os dados do utilizador
            Input.Utilizador.IdentityUserId = user.Id;
            Input.Utilizador.Email = user.Email;
            _context.Utilizador.Add(Input.Utilizador);
            
            // Atribui role de Cliente
            await _userManager.AddToRoleAsync(user, "Cliente");
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Utilizador criado com sucesso.");

            // Código de confirmação de email (existente)
            if (_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId, code, returnUrl },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(Input.Utilizador.Email, "Confirmação de Email",
                    $"Por favor confirme a sua conta <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicando aqui</a>.");

                return RedirectToPage("RegisterConfirmation", new { email = Input.Utilizador.Email, returnUrl });
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(returnUrl);
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        _logger.LogError(ex, "Erro durante o registo");
        ModelState.AddModelError(string.Empty, "Ocorreu um erro durante o registo.");
    }

    return Page();
}

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Não foi possível criar uma instância de '{nameof(IdentityUser)}'. " +
                    $"Certifique-se de que não é abstrata e tem um construtor sem parâmetros.");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("O sistema requer um user store com suporte a email.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
