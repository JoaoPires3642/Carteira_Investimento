using InvestidorCarteira.Application.DTOs;
using InvestidorCarteira.Application.Interfaces;
using InvestidorCarteira.Domain.Interfaces;

namespace InvestidorCarteira.Application.UseCases
{
    public class ObterDashboardUseCase : IObterDashboardUseCase
    {
        private readonly IPortfolioRepository _repository;
        private readonly IMarketDataService _marketService;

        public ObterDashboardUseCase(IPortfolioRepository repository, IMarketDataService marketService)
        {
            _repository = repository;
            _marketService = marketService;
        }

        public async Task<DashboardDto> Executar(Guid portfolioId)
        {
            // 1. Busca a carteira no banco
            var portfolio = await _repository.ObterPorIdAsync(portfolioId);
            if (portfolio == null) throw new Exception("Carteira não encontrada");

            var dashboard = new DashboardDto
            {
                NomeTitular = portfolio.NomeTitular
            };

            // 2. Para cada ativo, busca o preço atual no Yahoo
            foreach (var ativo in portfolio.Ativos)
            {
                var cotacao = await _marketService.ObterCotacaoAsync(ativo.Ticker);

                // Se a API falhar, usamos o Preço Médio como "Preço Atual" para não zerar o patrimônio visualmente
                decimal precoMercado = cotacao?.PrecoAtual ?? ativo.PrecoMedio;

                var item = new AtivoDashboardItem
                {
                    Ticker = ativo.Ticker,
                    Quantidade = (int)ativo.Quantidade,
                    PrecoMedio = ativo.PrecoMedio,
                    PrecoAtual = precoMercado,
                    DividendYield = cotacao?.DividendYield ?? 0,
                    VariacaoDoDia = cotacao?.VariacaoDiaria ?? 0
                };

                dashboard.Itens.Add(item);
            }

            // 3. Totais consolidados
            dashboard.TotalInvestido = dashboard.Itens.Sum(i => i.ValorInvestido);
            dashboard.PatrimonioAtual = dashboard.Itens.Sum(i => i.ValorAtual);

            return dashboard;
        }
    }
}