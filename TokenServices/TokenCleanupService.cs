using ProjetoFechadura.DAO;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

public class TokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _intervalo;

    public TokenCleanupService(IServiceProvider serviceProvider, TimeSpan intervalo)
    {
        _serviceProvider = serviceProvider;
        _intervalo = intervalo; // Define o intervalo de tempo entre as execuções
    }

    // Método que será executado em segundo plano
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var tokenDao = scope.ServiceProvider.GetRequiredService<TokenDAO>();
                Console.WriteLine("Tokens antes da limpeza:");
                tokenDao.LogAllTokens();
                
                CleanupExpiredTokens(tokenDao);
                
                Console.WriteLine("Tokens após a limpeza:");
                tokenDao.LogAllTokens();
            }

            // Aguarda o intervalo antes de executar novamente
            await Task.Delay(_intervalo, stoppingToken);
        }
    }

    // Método para remover tokens expirados
    private void CleanupExpiredTokens(TokenDAO tokenDao)
    {
        var tokensExpirados = tokenDao.ObterTokensExpirados();
        Console.WriteLine($"Tokens expirados encontrados: {tokensExpirados.Count}");
        foreach (var token in tokensExpirados)
        {
            Console.WriteLine($"Removendo token expirado: ID {token.IdToken}, Token: {token.Token}");
            tokenDao.RemoverToken(token.IdToken);
        }
        Console.WriteLine($"{tokensExpirados.Count} tokens expirados removidos.");
    }
}
