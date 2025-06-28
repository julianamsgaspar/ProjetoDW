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
    /// <summary>
    /// Classe responsável pelo processo de registo
    /// </summary>
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// // Construtor: recebe todos os serviços necessários
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="userStore"></param>
        /// <param name="signInManager"></param>
        /// <param name="logger"></param>
        /// <param name="emailSender"></param>
        /// <param name="context"></param>
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
        public InputModel Input { get; set; } // Dados do formulário de registo

        public string ReturnUrl { get; set; } // URL para onde redirecionar após sucesso
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            /*[Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }*/

            // Palavra-passe obrigatória, com mínimo de 6 caracteres
            [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
            [StringLength(100, ErrorMessage = "A palavra-passe deve ter entre {2} e {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            // Campo de confirmação de palavra-passe, tem de coincidir com a anterior
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Password")]
            [Compare("Password", ErrorMessage = "A password e a confirmação não coincidem.")]
            public string ConfirmPassword { get; set; }

            // Objeto do tipo Utilizador (modelo personalizado com dados adicionais)
            public Utilizador Utilizador { get; set; }
        }

        /// <summary>
        /// Método chamado quando a página é carregada
        /// </summary>
        /// <param name="returnUrl"></param>
        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        /// <summary>
        /// Método chamado quando o formulário é submetido
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
{
    _logger.LogInformation("Iniciando processo de registo..."); // <-- NOVO
    // Validação de modelo (dados mal preenchidos)
    if (!ModelState.IsValid)
    {
        _logger.LogWarning("ModelState inválido. Erros: " + 
                           string.Join(", ", ModelState.Values
                               .SelectMany(v => v.Errors)
                               .Select(e => e.ErrorMessage))); // <-- NOVO
        return Page(); // mostra os erros
    }
    returnUrl ??= Url.Content("~/");
    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

    if (!ModelState.IsValid) return Page();

    await using var transaction = await _context.Database.BeginTransactionAsync();
    
    try
    {
        var user = CreateUser();// cria um novo IdentityUser
        // 1. Verificar se o email já existe
        var existingUser = await _userManager.FindByEmailAsync(Input.Utilizador.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError("Input.Utilizador.Email", "Este email já está registado.");
            return Page();
        }
        
        // 2. Verificar se o username é igual ao email
        if (Input.Utilizador.Email.Equals(Input.Utilizador.Nome, StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError("Input.Utilizador.Nome", "O Nome não pode ser igual ao email.");
            return Page();
        }
        
        // 3. Verificar se o username já existe
        var existingUserByName = await _userManager.FindByNameAsync(Input.Utilizador.Nome);
        if (existingUserByName != null)
        {
            ModelState.AddModelError("Input.Utilizador.Nome", "Este Nome já está em uso.");
            return Page();
        }
        // Nome de utilizador não pode conter '@'
        if (Input.Utilizador.Nome.Contains("@"))
        {
            ModelState.AddModelError("Input.Utilizador.Nome", "O Nome não pode conter '@'.");
            return Page();
        }
        
        // Preenche o nome de utilizador e email no IdentityUser
        await _userStore.SetUserNameAsync(user, Input.Utilizador.Nome, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, Input.Utilizador.Email, CancellationToken.None);
        
        // Cria o utilizador na base de dados com a palavra-passe
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

                await _emailSender.SendEmailAsync(Input.Utilizador.Email, "Confirmação de Conta PawBuddy",
                    $@"
                    <div style='font-family: Arial, sans-serif; color: #333; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px;'>
                        <h2 style='color: #2E8B57;'>Olá!</h2>
                        <p>Obrigado por se registar no <strong>PawBuddy</strong>.</p>
                        <p>Para ativar sua conta, clique no botão abaixo:</p>
                        <p style='text-align: center; margin: 30px 0;'>
                            <a href='{HtmlEncoder.Default.Encode(callbackUrl)}' style='background-color: #2E8B57; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Confirmar Conta</a>
                        </p>
                        <p>Se você não se registou, por favor ignore este email.</p>
                        <br />
                        <p>Com os melhores cumprimentos,</p>
                        <p><strong>A equipa PawBuddy</strong></p>
                    </div>
                    ");



                return RedirectToPage("RegisterConfirmation", new { email = Input.Utilizador.Email, returnUrl });
            }

            // Inicia sessão do utilizador automaticamente
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
/// <summary>
/// Cria uma instância de IdentityUser
/// </summary>
/// <returns></returns>
/// <exception cref="InvalidOperationException"></exception>
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

        /// <summary>
        /// // Obtém o store que permite associar emails a utilizadores5
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
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
