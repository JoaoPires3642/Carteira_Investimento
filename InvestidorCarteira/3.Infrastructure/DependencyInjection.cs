using InvestidorCarteira.Infrastructure.Persistence;
using InvestidorCarteira.Domain.Interfaces;
using InvestidorCarteira.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InvestidorCarteira.Infrastructure.Services;

namespace InvestidorCarteira.Infrastructure
{
    public static class DependencyInjection
    {
        // "this IServiceCollection services" é o que torna isso um Extension Method
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Configura o Entity Framework para usar SQLite
            // O arquivo do banco vai se chamar "app.db" e ficará na pasta de execução
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite("Data Source=app.db"));

            // "Sempre que alguém pedir IPortfolioRepository, entregue o PortfolioRepository"
            // Scoped = Uma instância por requisição HTTP (o padrão para Repositórios)
             services.AddScoped<IPortfolioRepository, PortfolioRepository>();
             services.AddScoped<IMarketDataService, YahooFinanceService>();
             services.AddScoped<InvestidorCarteira.Application.Interfaces.IObterDashboardUseCase, InvestidorCarteira.Application.UseCases.ObterDashboardUseCase>();
            

            return services;
        }
    }
}