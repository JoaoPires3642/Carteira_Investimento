using InvestidorCarteira.API.DTOs;
using InvestidorCarteira.Domain.Entities;
using InvestidorCarteira.Domain.Enums;
using InvestidorCarteira.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvestidorCarteira.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

            return Ok(portfolio);
        }

        // POST: api/portfolio
        [HttpPost]
        public async Task<IActionResult> CriarCarteira([FromBody] string nomeTitular)
        {
            if (string.IsNullOrEmpty(nomeTitular))
                return BadRequest("O nome do titular é obrigatório.");

            var novaCarteira = new Portfolio(nomeTitular);

            await _repository.CriarAsync(novaCarteira);

            return CreatedAtAction(nameof(ObterPorId), new { id = novaCarteira.Id }, novaCarteira);
        }

        // POST: api/portfolio/{id}/comprar
        [HttpPost("{id}/comprar")]
        public async Task<IActionResult> ComprarAtivo(Guid id, [FromBody] ComprarAtivoRequest request)
        {
            // 1. Busca a Carteira
            var portfolio = await _repository.ObterPorIdAsync(id);

            if (portfolio == null)
                return NotFound("Carteira não encontrada.");

            // 2. Factory Pattern (Cria o Ativo correto)
            Ativos novoAtivo = request.Tipo switch
            {
                TipoAtivo.Acoes => new Acoes(request.Ticker, request.Quantidade, request.PrecoPago),
                TipoAtivo.FIIs => new FIIs(request.Ticker, request.Quantidade, request.PrecoPago),
                TipoAtivo.Criptomoedas => new Criptomoedas(request.Ticker, request.Quantidade, request.PrecoPago),
                TipoAtivo.ETFs => new ETFs(request.Ticker, request.Quantidade, request.PrecoPago),
                _ => null
            };

            if (novoAtivo == null)
                return BadRequest($"Tipo de ativo não suportado.");

            // 3. Chama a regra do Domínio
            portfolio.ComprarAtivo(novoAtivo);

            // 4. Salva (O EF Core identifica as mudanças nas tabelas Ativos e Transacoes)
            await _repository.AtualizarAsync(portfolio);

            return Ok(new { mensagem = "Compra registrada com sucesso!", ativoId = novoAtivo.Id });
        }

        // POST: api/portfolio/{id}/vender
        [HttpPost("{id}/vender")]
        public async Task<IActionResult> VenderAtivo(Guid id, [FromBody] VenderAtivoRequest request)
        {
            try
            {
                var portfolio = await _repository.ObterPorIdAsync(id);

                if (portfolio == null)
                    return NotFound("Carteira não encontrada.");

                // 2. Chama a regra do Domínio PASSANDO O PREÇO DE VENDA
                portfolio.VenderAtivo(request.Ticker, request.Quantidade, request.PrecoVenda);

                await _repository.AtualizarAsync(portfolio);

                return Ok(new { mensagem = "Venda registrada com sucesso!", ticker = request.Ticker });
            }
            catch (InvalidOperationException ex)
            {
                // Erros de regra de negócio (Saldo insuficiente, ativo não existe)
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Outros erros de regra de negócio
                return BadRequest(ex.Message);
            }
        }
    }
}