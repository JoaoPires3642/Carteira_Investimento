using InvestidorCarteira.Domain.Entities;
using InvestidorCarteira.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvestidorCarteira.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // A rota será: /api/portfolio
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioRepository _repository;

        public PortfolioController(IPortfolioRepository repository)
        {
            _repository = repository;
        }

        // GET: api/portfolio/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var portfolio = await _repository.ObterPorIdAsync(id);
            
            if (portfolio == null)
                return NotFound("Carteira não encontrada.");

            // Dica: Em produção, retornaríamos um DTO, não a Entidade direto.
            // Mas para aprender, vamos retornar o objeto completo.
            return Ok(portfolio);
        }

        // POST: api/portfolio
        [HttpPost]
        public async Task<IActionResult> CriarCarteira([FromBody] string nomeTitular)
        {
            // Validação simples
            if (string.IsNullOrEmpty(nomeTitular))
                return BadRequest("O nome do titular é obrigatório.");

            var novaCarteira = new Portfolio(nomeTitular);

            await _repository.CriarAsync(novaCarteira);

            // Retorna 201 Created e o link para consultar
            return CreatedAtAction(nameof(ObterPorId), new { id = novaCarteira.Id }, novaCarteira);
        }
    }
}