using InvestidorCarteira.Application.DTOs;
using InvestidorCarteira.Domain.Enums;
using InvestidorCarteira.Domain.Interfaces;

namespace InvestidorCarteira.Application.UseCases
{
    public class GerarRelatorioImpostoUseCase
    {
        private readonly IPortfolioRepository _repo;
        private readonly ApuracaoMensalUseCase _calculadora;

        // Injeção de Dependência via Construtor
        public GerarRelatorioImpostoUseCase(IPortfolioRepository repo, ApuracaoMensalUseCase calculadora)
        {
            _repo = repo;
            _calculadora = calculadora;
        }

        public async Task<ApuracaoResultadoDto> Executar(Guid portfolioId, int ano, int mes)
        {
            // 1. Busca o Portfolio JÁ COM AS TRANSAÇÕES (Include)
            var portfolio = await _repo.ObterPorIdComTransacoesAsync(portfolioId);

            if (portfolio == null)
                throw new Exception("Carteira não encontrada");

            // 2. Filtra as vendas do mês solicitado
            var operacoesDto = portfolio.Transacoes
                .Where(t => t.Data.Year == ano && 
                            t.Data.Month == mes && 
                            t.TipoOperacao == TipoOperacao.Venda)
                .Select(t => new OperationDto
                {
                    Ticker = t.Ticker,
                    Tipo = t.TipoAtivo,
                    Quantidade = t.Quantidade,
                    ValorVenda = t.Total, 
                    PrecoMedioHistorico = t.PrecoMedioNaData, // O PM gravado no momento da venda
                    Date = t.Data,
                    IsDayTrade = false // Futuramente implementar lógica de day trade
                })
                .ToList();

            // 3. Chama a lógica matemática (que já criamos antes)
            return _calculadora.Apurar(operacoesDto, ano, mes);
        }
    }
}