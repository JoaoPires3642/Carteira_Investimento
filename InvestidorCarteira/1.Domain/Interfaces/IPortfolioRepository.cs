using InvestidorCarteira.Domain.Entities;

namespace InvestidorCarteira.Domain.Interfaces
{
    public interface IPortfolioRepository
    {
        // O Task significa que será assíncrono e retornará uma promessa de resultado no futuro
        Task CriarAsync(Portfolio portfolio);
        Task<Portfolio?> ObterPorIdAsync(Guid id);
        Task AtualizarAsync(Portfolio portfolio);
        // Novo método para listar todos os portfolios
         Task<IEnumerable<Portfolio>> ListarTodosAsync();
    }
}