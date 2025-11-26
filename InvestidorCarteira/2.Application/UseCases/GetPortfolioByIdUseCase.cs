namespace InvestidorCarteira.Application.UseCases;
using InvestidorCarteira.Domain.Interfaces;
using InvestidorCarteira.Application.DTOs;
using InvestidorCarteira.Application.Mappers;
using System.Threading.Tasks;

public class GetPortfolioByIdUseCase
{
    private readonly IPortfolioRepository _repo;
    public GetPortfolioByIdUseCase(IPortfolioRepository repo)
    {
        _repo = repo;
    }

    public async Task<PortfolioDto> ExecuteAsync(string id)
    {
        var guid = Guid.Parse(id);
        var entity = await _repo.ObterPorIdAsync(guid);
        return entity is null ? new PortfolioDto(string.Empty, string.Empty) : PortfolioMapper.ToDto(entity);
    }
}   