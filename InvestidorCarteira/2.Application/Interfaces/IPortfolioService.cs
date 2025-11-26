using InvestidorCarteira.Application.DTOs;
using InvestidorCarteira.Domain.Enums;

namespace InvestidorCarteira.Application.Interfaces
{
    public interface IPortfolioService
    {
        Task<PortfolioDto?> ObterPorIdAsync(Guid id);
        Task<PortfolioDetailsDto?> ObterDetalhesPorIdAsync(Guid id);
        Task<PortfolioDto> CriarCarteiraAsync(string nomeTitular); // ou Task<PortfolioDto> CriarCarteiraAsync(CriarCarteiraDto request) se criar DTO em Application
        Task<OperacaoResponse> ComprarAtivoAsync(Guid portfolioId, TipoAtivo tipo, string ticker, decimal quantidade, decimal precoPago);
        Task<OperacaoResponse> VenderAtivoAsync(Guid portfolioId, string ticker, decimal quantidade, decimal precoVenda);
    }
}