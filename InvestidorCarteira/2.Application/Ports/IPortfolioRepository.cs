namespace InvestidorCarteira.Application.Ports;
using InvestidorCarteira.Domain.Entities;

public interface IPortfolioRepository
{
    Portfolio GetById(string id);
}