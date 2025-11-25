namespace InvestidorCarteira.Application.UseCases;
using InvestidorCarteira.Application.Ports;
using InvestidorCarteira.Application.DTOs;
using InvestidorCarteira.Application.Mappers;


public class GetPortfolioByIdUseCase
{

    private readonly IPortfolioRepository _repo;
    public GetPortfolioByIdUseCase(IPortfolioRepository repo)
    {
        _repo = repo;
    }
    public PortfolioDto Execute(string id)
    {
        var entity = _repo.GetById(id);
        return PortfolioMapper.ToDto(entity);
    }
}