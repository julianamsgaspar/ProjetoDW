using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using PawBuddy.Data;
using PawBuddy.Models;

namespace PawBuddy.Controllers;

/// <summary>
/// Controlador responsável pelas ações da página inicial e de privacidade do site PawBuddy.
/// </summary>
public class HomeController : Controller
{
    // Injeção de dependência para o serviço de logging
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IEmailSender _emailSender;

    public HomeController(
        ILogger<HomeController> logger,
        ApplicationDbContext context,
        IEmailSender emailSender)
    {
        _logger = logger;
        _context = context;
        _emailSender = emailSender;
    }
    
    /// <summary>
    /// Ação que retorna a view da página inicial.
    /// </summary>
    /// <returns>View da página Index</returns>
    public IActionResult Index()
    {
        if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "Administrador");
            // ou "Index", se for essa a action principal
        }
        var adotados = _context.Intencao
            .Count(i => i.Estado == EstadoAdocao.Concluido);

        ViewData["Adotados"] = adotados;
        return View();
    }
    public IActionResult QuemSomos()
    {
        return View();
    }
    /// <summary>
    /// Ação que retorna a view da página de privacidade.
    /// </summary>
    /// <returns>View da página Privacy</returns>
    public IActionResult Privacy()
    {
        return View();
    }
    /// <summary>
    /// Ação que trata erros e exibe a página de erro.
    /// Esta ação não armazena o cache da resposta.
    /// </summary>
    /// <returns>View com um modelo contendo o ID da requisição</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // Cria um modelo de erro com o ID da requisição atual ou, se nulo, o identificador de rastreamento do HTTP
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    /*public async Task<IActionResult> TestEmail()
    {
        await _emailSender.SendEmailAsync(
            "macielines3012@gmail.com",
            "Teste de Email - PawBuddy",
            "<h1>Funcionou!</h1><p>Este é um email de teste do PawBuddy.</p>"
        );

        return Content("Email enviado com sucesso.");
    }*/
}