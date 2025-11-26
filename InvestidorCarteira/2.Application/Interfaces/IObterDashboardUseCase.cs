using System;
using System.Threading.Tasks;
using InvestidorCarteira.Application.DTOs;

namespace InvestidorCarteira.Application.Interfaces
{
    public interface IObterDashboardUseCase
    {
        Task<DashboardDto> Executar(Guid portfolioId);
    }
}
