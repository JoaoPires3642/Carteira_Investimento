namespace InvestidorCarteira.Application.Mappers;
using InvestidorCarteira.Domain.Entities;
using InvestidorCarteira.Application.DTOs;
public static class PortfolioMapper
{
    public static PortfolioDto ToDto(Portfolio entity) => new(entity.Id, entity.Nome);
}