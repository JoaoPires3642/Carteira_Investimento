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
                .Include(p => p.Transacoes)  //registrar histórico
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AtualizarAsync(Portfolio portfolio)
        {
            
            foreach (var transacao in portfolio.Transacoes)
            {
                // Verifica se o EF já está rastreando essa transação
                var entry = _context.Entry(transacao);

                if (entry.State == EntityState.Unchanged) 
            continue;

        // Se o estado for 'Detached' (o EF não conhece), aí sim marcamos como Added.
        if (entry.State == EntityState.Detached)
        {
            entry.State = EntityState.Added;
        }
    }

    await _context.SaveChangesAsync();
}
        public Task<IEnumerable<Portfolio>> ListarTodosAsync()
        {
            throw new NotImplementedException();
        }
        public async Task<Portfolio?> ObterPorIdComTransacoesAsync(Guid id)
{
    return await _context.Portfolios
        .Include(p => p.Transacoes) 
        .FirstOrDefaultAsync(p => p.Id == id);
}
    }
}