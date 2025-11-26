using InvestidorCarteira.Application.Interfaces;
using InvestidorCarteira.Application.UseCases;
using InvestidorCarteira.Application.Services;
using InvestidorCarteira.Infrastructure; // Importante para achar a Injeção
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurações Regionais (R$)
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");

// 2. Adiciona serviços ao container.
builder.Services.AddControllers()
.AddJsonOptions(options =>
    {
        // Permite enviar "Acoes" no JSON em vez de 0
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Adiciona o Swagger (Documentação automática)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Conectamos sua Infraestrutura (Banco SQLite) na API
builder.Services.AddInfrastructure(); 

// Registra os UseCases
builder.Services.AddScoped<ApuracaoMensalUseCase>();
builder.Services.AddScoped<IGerarRelatorioImpostoUseCase, GerarRelatorioImpostoUseCase>();
builder.Services.AddScoped<GetPortfolioByIdUseCase>();
builder.Services.AddScoped<IPortfolioService, PortfolioService>();
// ---------------------------

var app = builder.Build();

// 3. Configura o Pipeline de requisições
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<InvestidorCarteira.Infrastructure.Persistence.AppDbContext>();
    // Isso aplica qualquer migration pendente automaticamente ao iniciar a API
    dbContext.Database.Migrate();
}

app.Run();