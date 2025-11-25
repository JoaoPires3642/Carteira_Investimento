using InvestidorCarteira.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvestidorCarteira.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RelatorioController : ControllerBase
    {
        private readonly IGerarRelatorioImpostoUseCase _useCase;

        public RelatorioController(IGerarRelatorioImpostoUseCase useCase)
        {
            _useCase = useCase;
        }

        // GET: api/relatorio/{idCarteira}/2023/10 (Outubro de 2023)
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
    }
}