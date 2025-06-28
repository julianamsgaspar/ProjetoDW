using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

public class ConfirmEmailModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public ConfirmEmailModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return RedirectToPage("/Index");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound($"Não foi possível carregar o utilizador com ID '{userId}'.");
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, code);

        StatusMessage = result.Succeeded ? "Email confirmado com sucesso!" : "Erro ao confirmar o email.";
        return Page();
    }
}