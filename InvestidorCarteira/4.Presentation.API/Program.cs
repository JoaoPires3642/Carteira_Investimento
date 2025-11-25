using InvestidorCarteira.Infrastructure; // Importante para achar a Injeção
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurações Regionais (R$)
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");

// 2. Adiciona serviços ao container.
builder.Services.AddControllers();

// Adiciona o Swagger (Documentação automática)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Conectamos sua Infraestrutura (Banco SQLite) na API
builder.Services.AddInfrastructure(); 
// ---------------------------

var app = builder.Build();

// 3. Configura o Pipeline de requisições
if (app.Environment.IsDevelopment())
{
    // Ativa o Swagger no modo Desenvolvimento
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<InvestidorCarteira.Infrastructure.Persistence.AppDbContext>();
    // Isso aplica qualquer migration pendente automaticamente ao iniciar a API
    dbContext.Database.Migrate();
}

app.Run();