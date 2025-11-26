using InvestidorCarteira.Application.DTOs;
using InvestidorCarteira.Application.Interfaces;
using InvestidorCarteira.Application.Mappers;
using InvestidorCarteira.Domain.Entities;
using InvestidorCarteira.Domain.Enums;
using InvestidorCarteira.Domain.Interfaces;

namespace InvestidorCarteira.Application.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IPortfolioRepository _repo;

        public PortfolioService(IPortfolioRepository repo)
        {
            _repo = repo;
        }

        public async Task<PortfolioDto?> ObterPorIdAsync(Guid id)
        {
            var portfolio = await _repo.ObterPorIdAsync(id);
            return portfolio is null ? null : PortfolioMapper.ToDto(portfolio);
        }

        public async Task<PortfolioDetailsDto?> ObterDetalhesPorIdAsync(Guid id)
        {
            var portfolio = await _repo.ObterPorIdAsync(id);
            return portfolio is null ? null : PortfolioMapper.ToDetailsDto(portfolio);
        }

        public async Task<PortfolioDto> CriarCarteiraAsync(string nomeTitular)
        {
            var portfolio = new Portfolio(nomeTitular);
            await _repo.CriarAsync(portfolio);
            return PortfolioMapper.ToDto(portfolio);
        }

        public async Task<OperacaoResponse> ComprarAtivoAsync(Guid portfolioId, TipoAtivo tipo, string ticker, decimal quantidade, decimal precoPago)
        {
            var portfolio = await _repo.ObterPorIdAsync(portfolioId) ?? throw new InvalidOperationException("Carteira não encontrada.");

            Ativos novoAtivo = tipo switch
            {
                TipoAtivo.Acoes => new Acoes(ticker, quantidade, precoPago),
                TipoAtivo.FIIs => new FIIs(ticker, quantidade, precoPago),
                TipoAtivo.Criptomoedas => new Criptomoedas(ticker, quantidade, precoPago),
                TipoAtivo.ETFs => new ETFs(ticker, quantidade, precoPago),
                _ => throw new InvalidOperationException("Tipo de ativo inválido.")
            };

            var pmAntes = portfolio.Ativos.FirstOrDefault(a => a.Ticker == ticker.ToUpper())?.PrecoMedio ?? 0m;

            portfolio.ComprarAtivo(novoAtivo);
            await _repo.AtualizarAsync(portfolio);

            return new OperacaoResponse(
                ticker.ToUpper(),
                tipo,
                TipoOperacao.Compra,
                quantidade,
                precoPago,
                DateTime.UtcNow,
                pmAntes
            );
        }

        public async Task<OperacaoResponse> VenderAtivoAsync(Guid portfolioId, string ticker, decimal quantidade, decimal precoVenda)
        {
            var portfolio = await _repo.ObterPorIdAsync(portfolioId) ?? throw new InvalidOperationException("Carteira não encontrada.");

            var ativo = portfolio.Ativos.FirstOrDefault(a => a.Ticker == ticker.ToUpper()) ?? throw new InvalidOperationException("Ativo não encontrado.");
            var pmNoMomento = ativo.PrecoMedio;

            portfolio.VenderAtivo(ticker, quantidade, precoVenda);
            await _repo.AtualizarAsync(portfolio);

            var tipoAtivo = ativo is Acoes ? TipoAtivo.Acoes :
                            ativo is FIIs ? TipoAtivo.FIIs :
                            ativo is Criptomoedas ? TipoAtivo.Criptomoedas :
                            TipoAtivo.ETFs;

            return new OperacaoResponse(
                ticker.ToUpper(),
                tipoAtivo,
                TipoOperacao.Venda,
                quantidade,
                precoVenda,
                DateTime.UtcNow,
                pmNoMomento
            );
        }
    }
}