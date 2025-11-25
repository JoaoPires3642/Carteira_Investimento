namespace InvestidorCarteira.Application.Ports;
using InvestidorCarteira.Domain.Entities;
// Quanto esta com I atraz do nome, é uma interface
public interface IPortfolioRepository
{
    Portfolio GetById(string id);
}