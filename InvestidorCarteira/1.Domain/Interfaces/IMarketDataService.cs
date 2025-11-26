using InvestidorCarteira.Domain.ValueObjects;

namespace InvestidorCarteira.Domain.Interfaces
{
    public interface IMarketDataService
    {
        // Busca preço, fechamento e yield de um único ativo
        Task<Cotacao?> ObterCotacaoAsync(string ticker);

        //Para buscar vários de uma vez (performance mas Precisa de API paga geralmente)
        // Task<IEnumerable<Cotacao>> ObterCotacoesEmLoteAsync(IEnumerable<string> tickers);
    }
}