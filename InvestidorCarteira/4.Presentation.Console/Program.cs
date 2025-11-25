using InvestidorCarteira.Application.UseCases;
using InvestidorCarteira.Infrastructure.Repositories;
var repo = new InMemoryPortfolioRepository();
var useCase = new GetPortfolioByIdUseCase(repo);
var dto = useCase.Execute("1");
System.Console.WriteLine($"{dto.Id} - {dto.Nome}");