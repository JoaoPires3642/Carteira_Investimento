using InvestidorCarteira.Domain.Entities;
using InvestidorCarteira.Domain.Interfaces;
using InvestidorCarteira.Infrastructure; // Para usar o .AddInfrastructure()
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

// 1. CONFIGURAÇÃO DA INJEÇÃO DE DEPENDÊNCIA (DI)
// criando uma coleção de serviços, igual uma API faria
var services = new ServiceCollection();

// Força a cultura para Brasil (R$) independente do idioma do PC/Servidor
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");

// Aqui usamos aquele método que criamos na Infra. 
// Ele já configura o SQLite e o Repositório de uma vez só.
services.AddInfrastructure();

// Constrói o provedor 
var serviceProvider = services.BuildServiceProvider();

Console.WriteLine("--- INICIANDO SISTEMA DE CARTEIRA ---");

// 2. SIMULANDO O USO DO SISTEMA
// Criamos um escopo (como se fosse uma requisição web)
using (var scope = serviceProvider.CreateScope())
{
    // Pedimos o Repositório para a Injeção de Dependência
    var repositorio = scope.ServiceProvider.GetRequiredService<IPortfolioRepository>();

    // CENÁRIO 1: CRIAR E SALVAR
    Console.WriteLine("\n1. Criando nova carteira...");
    
    var minhaCarteira = new Portfolio("Carteira do João - Aposentadoria");
    
    // Adicionando Ativos 
    minhaCarteira.AdicionarAtivo(new Acoes("PETR4", 100, 32.50m));           // R$ 3.250
    minhaCarteira.AdicionarAtivo(new Acoes("BBSA3", 50, 120.00m));          // R$ 6.000
    minhaCarteira.AdicionarAtivo(new FIIs("HGLG11", 10, 160.00m)); // R$ 1.600
    minhaCarteira.AdicionarAtivo(new Criptomoedas("BTC", 0.005m, 350000.00m));    // R$ 1.750

    Console.WriteLine($"   Salvando carteira '{minhaCarteira.NomeTitular}' no banco...");
    await repositorio.CriarAsync(minhaCarteira);
    
    Console.WriteLine("   >> Carteira Salva com Sucesso! ID: " + minhaCarteira.Id);
    
    // Guardamos o ID para buscar depois
    var idCarteira = minhaCarteira.Id;

    // ---------------------------------------------------------
    // CENÁRIO 2: BUSCAR DO BANCO (Para provar que persistiu)
    // ---------------------------------------------------------
    Console.WriteLine("\n2. Buscando dados do SQLite...");
    
    // Limpamos o contexto para garantir que vem do banco e não da memória
    // (Em uma API isso acontece automaticamente a cada request)
    scope.ServiceProvider.GetRequiredService<InvestidorCarteira.Infrastructure.Persistence.AppDbContext>().ChangeTracker.Clear();

    var carteiraDoBanco = await repositorio.ObterPorIdAsync(idCarteira);

    if (carteiraDoBanco != null)
    {
        Console.WriteLine($"\n   RESUMO DA CARTEIRA RECUPERADA:");
        Console.WriteLine($"   Titular: {carteiraDoBanco.NomeTitular}");
        Console.WriteLine($"   Total de Ativos: {carteiraDoBanco.Ativos.Count}");
        
        foreach (var ativo in carteiraDoBanco.Ativos)
        {
            // O GetType().Name vai mostrar "Acao", "FundoImobiliario" ou "Cripto"
            Console.WriteLine($"   - [{ativo.GetType().Name}] {ativo.Ticker}: {ativo.Quantidade} un. (Médio: R$ {ativo.PrecoMedio})");
        }

        // Teste de cálculo usando o método do domínio
        var patrimonio = carteiraDoBanco.CalcularPatrimonioTotal();
        Console.WriteLine($"\n   PATRIMÔNIO TOTAL ESTIMADO: {patrimonio:C}");
    }
    else
    {
        Console.WriteLine("   ERRO: Carteira não encontrada!");
    }
}

Console.WriteLine("\n--- FIM ---");