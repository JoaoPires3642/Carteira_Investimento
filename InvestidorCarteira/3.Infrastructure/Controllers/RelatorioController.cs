using InvestidorCarteira.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvestidorCarteira.Infrastructure.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RelatorioController : ControllerBase
    {
        private readonly IGerarRelatorioImpostoUseCase _useCase;
        private readonly IObterDashboardUseCase _dashboardUseCase;

        public RelatorioController(IGerarRelatorioImpostoUseCase useCase, IObterDashboardUseCase dashboardUseCase)
        {
            _useCase = useCase;
            _dashboardUseCase = dashboardUseCase;
        }

        [HttpGet("{portfolioId}/{ano}/{mes}")]
        public async Task<IActionResult> ObterRelatorioMensal(Guid portfolioId, int ano, int mes)
        {
            try
            {
                var resultado = await _useCase.Executar(portfolioId, ano, mes);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        [HttpGet("{portfolioId}/dashboard")]
public async Task<IActionResult> ObterDashboard(Guid portfolioId)
{
    try 
    {
        var result = await _dashboardUseCase.Executar(portfolioId);
        return Ok(result);
    }
    catch (Exception ex)
    {
        return BadRequest(ex.Message);
    }
}
    }
}