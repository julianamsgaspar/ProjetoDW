// BackgroundService/AdocaoBackgroundService.cs
using Microsoft.EntityFrameworkCore;
using PawBuddy.Data;
using PawBuddy.Models;

public class AdocaoBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<AdocaoBackgroundService> _logger;

    public AdocaoBackgroundService(IServiceProvider services, ILogger<AdocaoBackgroundService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AdocaoBackgroundService is running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                // Verificar intenções em validação com mais de 24h
                var intencoesParaValidar = await context.Intencao
                    .Where(i => i.Estado == EstadoAdocao.Emvalidacao && 
                                i.DataIA < DateTime.Now.AddHours(-24))
                    .ToListAsync();

                foreach (var intencao in intencoesParaValidar)
                {
                    // Se só existe esta intenção, conclui automaticamente
                    var outrasIntencoes = await context.Intencao
                        .Where(i => i.AnimalFK == intencao.AnimalFK && 
                                   i.Id != intencao.Id && 
                                   i.Estado != EstadoAdocao.Rejeitado)
                        .CountAsync();
                    

                    if (outrasIntencoes == 0)
                    {
                        await ConcluirAdocao(context, intencao);
                    }
                    else
                    {
                        
                        // Muda para Emprocesso se houver outras intenções
                        intencao.Estado = EstadoAdocao.Emprocesso;
                        context.Update(intencao);
                    }
                }

                await context.SaveChangesAsync();
            }

            // Verifica a cada hora
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task ConcluirAdocao(ApplicationDbContext context, IntencaoDeAdocao intencao)
    {
        var adotam = new Adotam
        {
            AnimalFK = intencao.AnimalFK,
            UtilizadorFK = intencao.UtilizadorFK,
            dateA = DateTime.Now
        };
        context.Adotam.Add(adotam);
        context.Intencao.Remove(intencao);
        await context.SaveChangesAsync();
    }
}