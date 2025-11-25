using InvestidorCarteira.Domain.Entities;
using InvestidorCarteira.Application.DTOs;

namespace InvestidorCarteira.Application.Mappers;

public static class PortfolioMapper
{
    // Converte entidade `Portfolio` para `PortfolioDto`.
    public static PortfolioDto ToDto(Portfolio entity)
        => new(entity.Id.ToString(), entity.NomeTitular);
}