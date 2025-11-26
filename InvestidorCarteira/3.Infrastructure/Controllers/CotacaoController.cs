using InvestidorCarteira.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvestidorCarteira.Infrastructure.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CotacaoController : ControllerBase
    {
        private readonly IMarketDataService _marketService;

        public CotacaoController(IMarketDataService marketService)
        {
            _marketService = marketService;
        }

        [HttpGet("{ticker}")]
        public async Task<IActionResult> ConsultarPreco(string ticker)
        {
            var cotacao = await _marketService.ObterCotacaoAsync(ticker);

            if (cotacao == null)
                return NotFound($"Ativo '{ticker}' não encontrado ou API indisponível.");

            return Ok(cotacao);
        }
    }
}