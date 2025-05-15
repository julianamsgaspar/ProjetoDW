using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PawBuddy.Models;

namespace PawBuddy.Controllers;

/// <summary>
/// Controlador responsável pelas ações da página inicial e de privacidade do site PawBuddy.
/// </summary>
public class HomeController : Controller
{
    // Injeção de dependência para o serviço de logging
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Construtor que recebe o logger para registrar logs da aplicação.
    /// </summary>
    /// <param name="logger">Instância do logger injetado pelo framework</param>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Ação que retorna a view da página inicial.
    /// </summary>
    /// <returns>View da página Index</returns>
    public IActionResult Index()
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
}