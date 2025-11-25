using InvestidorCarteira.Domain.Entities;
using InvestidorCarteira.Domain.Interfaces;
using InvestidorCarteira.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvestidorCarteira.Infrastructure.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly AppDbContext _context;

        // Injeção de Dependência: O Repositório pede o Contexto
        public PortfolioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CriarAsync(Portfolio portfolio)
        {
            // Adiciona na memória
            await _context.Portfolios.AddAsync(portfolio);
            // Comita no banco de dados 
            await _context.SaveChangesAsync();
        }

        public async Task<Portfolio?> ObterPorIdAsync(Guid id)
        {
            // ATENÇÃO AQUI: .Include()
            // Sem isso, a lista de ativos viria vazia.
            return await _context.Portfolios
                .Include(p => p.Ativos) 
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AtualizarAsync(Portfolio portfolio)
        {
            _context.Portfolios.Update(portfolio);
            await _context.SaveChangesAsync();
        }

        public Task<IEnumerable<Portfolio>> ListarTodosAsync()
        {
            throw new NotImplementedException();
        }
    }
}