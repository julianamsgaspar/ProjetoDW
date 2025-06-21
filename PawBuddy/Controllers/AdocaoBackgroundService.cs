// BackgroundService/AdocaoBackgroundService.cs
using Microsoft.EntityFrameworkCore;
using PawBuddy.Data;
using PawBuddy.Models;


/// <summary>
/// Serviço em segundo plano para gestão automática de processos de adoção.
/// Verifica periodicamente intenções de adoção e toma ações conforme necessário.
/// </summary>
public class AdocaoBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services; // Provedor de serviços para DI
    private readonly ILogger<AdocaoBackgroundService> _logger;// Logger para registo de atividades

    /// <summary>
    /// Construtor do serviço de adoção em segundo plano
    /// </summary>
    /// <param name="services">Provedor de serviços para injeção de dependências</param>
    /// <param name="logger">Logger para registo de atividades</param>
    public AdocaoBackgroundService(IServiceProvider services, ILogger<AdocaoBackgroundService> logger)
    {
        _services = services;
        _logger = logger;
    }

    /// <summary>
    /// Método principal de execução do serviço em segundo plano.
    /// Executa verificações periódicas nas intenções de adoção.
    /// </summary>
    /// <param name="stoppingToken">Token de cancelamento para parar o serviço</param>
    /// <returns>Task representando a operação assíncrona</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Registo de início
        _logger.LogInformation("AdocaoBackgroundService is running.");

        // Loop principal enquanto o serviço estiver ativo
        while (!stoppingToken.IsCancellationRequested)
        {
            // Criar um scope para obter serviços com vida curta
            using (var scope = _services.CreateScope())
            {
                // Obter o contexto da base de dados
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                // Obter intenções em validação com mais de 24h
                var intencoesParaValidar = await context.Intencao
                    .Where(i => i.Estado == EstadoAdocao.Emvalidacao && 
                                i.DataIA < DateTime.Now.AddHours(-24))
                    .ToListAsync();

                // Processar cada intenção encontrada
                foreach (var intencao in intencoesParaValidar)
                {
                    // Verificar se existem outras intenções para o mesmo animal
                    var outrasIntencoes = await context.Intencao
                        .Where(i => i.AnimalFK == intencao.AnimalFK && // Mesmo animal
                                   i.Id != intencao.Id &&  // Excluir a própria intenção
                                   i.Estado != EstadoAdocao.Rejeitado) // Excluir rejeitadas
                        .CountAsync();
                    

                    // Se não houver outras intenções, concluir automaticamente
                    if (outrasIntencoes == 0)
                    {
                        await ConcluirAdocao(context, intencao);
                    }
                    else
                    {
                        // Se houver outras intenções, mudar estado para "Em processo"
                        intencao.Estado = EstadoAdocao.Emprocesso;
                        context.Update(intencao);
                    }
                }
                
                // Guardar todas as alterações na base de dados
                await context.SaveChangesAsync();
            }

            // Esperar 1 hora antes da próxima verificação
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    
    /// <summary>
    /// Conclui automaticamente um processo de adoção criando o registo definitivo
    /// e removendo a intenção original
    /// </summary>
    /// <param name="context">Contexto da base de dados</param>
    /// <param name="intencao">Intenção de adoção a processar</param>
    /// <returns>Task representando a operação assíncrona</returns>
    private async Task ConcluirAdocao(ApplicationDbContext context, IntencaoDeAdocao intencao)
    {
        // Criar novo registo de adoção
        var adotam = new Adotam
        {
            AnimalFK = intencao.AnimalFK,// ID do animal
            UtilizadorFK = intencao.UtilizadorFK,// ID do utilizador
            dateA = DateTime.Now // Data atual
        };
        context.Adotam.Add(adotam); // Adicionar à base de dados
        context.Intencao.Remove(intencao); // Remover a intenção de adoção
        await context.SaveChangesAsync();// Guardar alterações
    }
}