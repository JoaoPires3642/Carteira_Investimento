namespace InvestidorCarteira.Infrastructure.Repositories;
using InvestidorCarteira.Application.Ports;
using InvestidorCarteira.Domain.Entities;





public class InMemoryPortfolioRepository : IPortfolioRepository
{
    private readonly Dictionary<string, Portfolio> _store = new();
    public InMemoryPortfolioRepository()
    {
        _store["1"] = new Portfolio("1", "Minha Carteira");
    }
    public Portfolio GetById(string id)
    {
        if (_store.TryGetValue(id, out var p)) return p;
        return new Portfolio(id, "Carteira");
    }
}