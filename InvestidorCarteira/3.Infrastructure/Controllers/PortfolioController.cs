using InvestidorCarteira.Infrastructure.DTOs;
using InvestidorCarteira.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvestidorCarteira.Infrastructure.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _service;

        public PortfolioController(IPortfolioService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var dto = await _service.ObterPorIdAsync(id);
            if (dto == null) return NotFound("Carteira não encontrada.");
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CriarCarteira([FromBody] CriarCarteiraRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.NomeTitular))
                return BadRequest("O nome do titular é obrigatório.");
            var created = await _service.CriarCarteiraAsync(request.NomeTitular);
            return CreatedAtAction(nameof(ObterPorId), new { id = created.Id }, created);
        }

        [HttpPost("{id}/comprar")]
        public async Task<IActionResult> ComprarAtivo(Guid id, [FromBody] ComprarAtivoRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _service.ComprarAtivoAsync(id, request.Tipo, request.Ticker, request.Quantidade, request.PrecoPago);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPost("{id}/vender")]
        public async Task<IActionResult> VenderAtivo(Guid id, [FromBody] VenderAtivoRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _service.VenderAtivoAsync(id, request.Ticker, request.Quantidade, request.PrecoVenda);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("{id}/completo")]
        public async Task<IActionResult> ObterPorIdCompleto(Guid id)
        {
            var detalhes = await _service.ObterDetalhesPorIdAsync(id);
            if (detalhes == null) return NotFound("Carteira não encontrada.");
            return Ok(detalhes);
        }
    }
}