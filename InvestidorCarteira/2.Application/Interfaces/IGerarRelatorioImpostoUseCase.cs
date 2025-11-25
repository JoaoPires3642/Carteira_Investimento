using InvestidorCarteira.Application.DTOs;

namespace InvestidorCarteira.Application.Interfaces
{
    public interface IGerarRelatorioImpostoUseCase
    {
        Task<ApuracaoResultadoDto> Executar(Guid portfolioId, int ano, int mes);
    }
}
