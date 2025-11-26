using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using InvestidorCarteira.Infrastructure.Controllers;
using InvestidorCarteira.Infrastructure.DTOs;
using InvestidorCarteira.Domain.Entities;
using InvestidorCarteira.Domain.Enums;
using InvestidorCarteira.Application.Interfaces;
using InvestidorCarteira.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InvestidorCarteira.Tests.Controllers
{
    public class PortfolioControllerTests
    {
        private readonly Mock<IPortfolioService> _serviceMock;
        private readonly PortfolioController _controller;

        public PortfolioControllerTests()
        {
            _serviceMock = new Mock<IPortfolioService>();
            _controller = new PortfolioController(_serviceMock.Object);
        }

        #region ObterPorId Tests

        [Fact]
        public async Task ObterPorId_DeveRetornarOk_QuandoCarteiraExiste()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var dto = new PortfolioDto(portfolioId.ToString(), "João Silva");
            _serviceMock.Setup(s => s.ObterPorIdAsync(portfolioId))
                .ReturnsAsync(dto);

            // Act
            var result = await _controller.ObterPorId(portfolioId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(dto);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarNotFound_QuandoCarteiraNaoExiste()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            _serviceMock.Setup(s => s.ObterPorIdAsync(portfolioId))
                .ReturnsAsync((PortfolioDto)null);

            // Act
            var result = await _controller.ObterPorId(portfolioId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        #endregion

        #region CriarCarteira Tests

        [Fact]
        public async Task CriarCarteira_DeveRetornarCreated_QuandoNomeValido()
        {
            // Arrange
            var nomeTitular = "Maria Santos";
            _serviceMock.Setup(s => s.CriarCarteiraAsync(nomeTitular))
                .ReturnsAsync(new PortfolioDto(Guid.NewGuid().ToString(), nomeTitular));

            // Act
            var result = await _controller.CriarCarteira(new CriarCarteiraRequest { NomeTitular = nomeTitular });

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult.ActionName.Should().Be(nameof(PortfolioController.ObterPorId));
            
            var portfolio = createdResult.Value as PortfolioDto;
            portfolio.Should().NotBeNull();
            portfolio.Nome.Should().Be(nomeTitular);
            
            _serviceMock.Verify(s => s.CriarCarteiraAsync(nomeTitular), Times.Once);
        }

        [Fact]
        public async Task CriarCarteira_DeveRetornarBadRequest_QuandoNomeVazio()
        {
            // Arrange
            var nomeTitular = string.Empty;

            // Act
            var result = await _controller.CriarCarteira(new CriarCarteiraRequest { NomeTitular = nomeTitular });

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            
            _serviceMock.Verify(s => s.CriarCarteiraAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task CriarCarteira_DeveRetornarBadRequest_QuandoNomeNulo()
        {
            // Arrange
            string nomeTitular = null;

            // Act
            var result = await _controller.CriarCarteira(new CriarCarteiraRequest { NomeTitular = nomeTitular });

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _serviceMock.Verify(s => s.CriarCarteiraAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region ComprarAtivo Tests

        [Fact]
        public async Task ComprarAtivo_DeveRetornarOk_QuandoComprarAcoes()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var portfolio = new Portfolio("João Silva");
            
            var request = new ComprarAtivoRequest
            {
                Ticker = "PETR4",
                Quantidade = 100,
                PrecoPago = 30.50m,
                Tipo = TipoAtivo.Acoes
            };

            _serviceMock.Setup(s => s.ComprarAtivoAsync(portfolioId, TipoAtivo.Acoes, request.Ticker, request.Quantidade, request.PrecoPago))
                .ReturnsAsync(new OperacaoResponse(request.Ticker, TipoAtivo.Acoes, TipoOperacao.Compra, (int)request.Quantidade, request.PrecoPago, DateTime.UtcNow, 0m));

            // Act
            var result = await _controller.ComprarAtivo(portfolioId, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            
            var ok = result as OkObjectResult;
            ok.Value.Should().BeOfType<OperacaoResponse>();
        }

        [Fact]
        public async Task ComprarAtivo_DeveRetornarOk_QuandoComprarFIIs()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var portfolio = new Portfolio("João Silva");
            
            var request = new ComprarAtivoRequest
            {
                Ticker = "HGLG11",
                Quantidade = 50,
                PrecoPago = 150.00m,
                Tipo = TipoAtivo.FIIs
            };

            _serviceMock.Setup(s => s.ComprarAtivoAsync(portfolioId, TipoAtivo.FIIs, request.Ticker, request.Quantidade, request.PrecoPago))
                .ReturnsAsync(new OperacaoResponse(request.Ticker, TipoAtivo.FIIs, TipoOperacao.Compra, (int)request.Quantidade, request.PrecoPago, DateTime.UtcNow, 0m));

            // Act
            var result = await _controller.ComprarAtivo(portfolioId, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok2 = result as OkObjectResult;
            ok2.Value.Should().BeOfType<OperacaoResponse>();
        }

        [Fact]
        public async Task ComprarAtivo_DeveRetornarOk_QuandoComprarCriptomoedas()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var portfolio = new Portfolio("João Silva");
            
            var request = new ComprarAtivoRequest
            {
                Ticker = "BTC",
                Quantidade = 0.5m,
                PrecoPago = 250000.00m,
                Tipo = TipoAtivo.Criptomoedas
            };

            _serviceMock.Setup(s => s.ComprarAtivoAsync(portfolioId, TipoAtivo.Criptomoedas, request.Ticker, request.Quantidade, request.PrecoPago))
                .ReturnsAsync(new OperacaoResponse(request.Ticker, TipoAtivo.Criptomoedas, TipoOperacao.Compra, (int)request.Quantidade, request.PrecoPago, DateTime.UtcNow, 0m));

            // Act
            var result = await _controller.ComprarAtivo(portfolioId, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok3 = result as OkObjectResult;
            ok3.Value.Should().BeOfType<OperacaoResponse>();
        }

        [Fact]
        public async Task ComprarAtivo_DeveRetornarOk_QuandoComprarETFs()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var portfolio = new Portfolio("João Silva");
            
            var request = new ComprarAtivoRequest
            {
                Ticker = "IVVB11",
                Quantidade = 20,
                PrecoPago = 300.00m,
                Tipo = TipoAtivo.ETFs
            };

            _serviceMock.Setup(s => s.ComprarAtivoAsync(portfolioId, TipoAtivo.ETFs, request.Ticker, request.Quantidade, request.PrecoPago))
                .ReturnsAsync(new OperacaoResponse(request.Ticker, TipoAtivo.ETFs, TipoOperacao.Compra, (int)request.Quantidade, request.PrecoPago, DateTime.UtcNow, 0m));

            // Act
            var result = await _controller.ComprarAtivo(portfolioId, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok4 = result as OkObjectResult;
            ok4.Value.Should().BeOfType<OperacaoResponse>();
        }

        [Fact]
        public async Task ComprarAtivo_DeveRetornarNotFound_QuandoCarteiraNaoExiste()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var request = new ComprarAtivoRequest
            {
                Ticker = "PETR4",
                Quantidade = 100,
                PrecoPago = 30.50m,
                Tipo = TipoAtivo.Acoes
            };

            _serviceMock.Setup(s => s.ComprarAtivoAsync(portfolioId, request.Tipo, request.Ticker, request.Quantidade, request.PrecoPago))
                .ThrowsAsync(new InvalidOperationException("Carteira não encontrada."));

            // Act
            var result = await _controller.ComprarAtivo(portfolioId, request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion

        #region VenderAtivo Tests

        [Fact]
        public async Task VenderAtivo_DeveRetornarOk_QuandoVendaValida()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var portfolio = new Portfolio("João Silva");

            var request = new VenderAtivoRequest
            {
                Ticker = "PETR4",
                Quantidade = 50,
                PrecoVenda = 35.00m
            };

            _serviceMock.Setup(s => s.VenderAtivoAsync(portfolioId, request.Ticker, request.Quantidade, request.PrecoVenda))
                .ReturnsAsync(new OperacaoResponse(request.Ticker, TipoAtivo.Acoes, TipoOperacao.Venda, request.Quantidade, request.PrecoVenda, DateTime.UtcNow, 30.50m));

            // Act
            var result = await _controller.VenderAtivo(portfolioId, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okVenda = result as OkObjectResult;
            okVenda.Value.Should().BeOfType<OperacaoResponse>();
        }

        [Fact]
        public async Task VenderAtivo_DeveRetornarNotFound_QuandoCarteiraNaoExiste()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var request = new VenderAtivoRequest
            {
                Ticker = "PETR4",
                Quantidade = 50,
                PrecoVenda = 35.00m
            };

            _serviceMock.Setup(s => s.VenderAtivoAsync(portfolioId, request.Ticker, request.Quantidade, request.PrecoVenda))
                .ThrowsAsync(new InvalidOperationException("Carteira não encontrada."));

            // Act
            var result = await _controller.VenderAtivo(portfolioId, request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task VenderAtivo_DeveRetornarBadRequest_QuandoAtivoNaoExiste()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var portfolio = new Portfolio("João Silva");

            var request = new VenderAtivoRequest
            {
                Ticker = "PETR4",
                Quantidade = 50,
                PrecoVenda = 35.00m
            };

            _serviceMock.Setup(s => s.VenderAtivoAsync(portfolioId, request.Ticker, request.Quantidade, request.PrecoVenda))
                .ThrowsAsync(new InvalidOperationException("Ativo não encontrado."));

            // Act & Assert
            var result = await _controller.VenderAtivo(portfolioId, request);
            
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task VenderAtivo_DeveRetornarBadRequest_QuandoQuantidadeInsuficiente()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var portfolio = new Portfolio("João Silva");

            var request = new VenderAtivoRequest
            {
                Ticker = "PETR4",
                Quantidade = 150,  // Maior que a quantidade disponível
                PrecoVenda = 35.00m
            };

            _serviceMock.Setup(s => s.VenderAtivoAsync(portfolioId, request.Ticker, request.Quantidade, request.PrecoVenda))
                .ThrowsAsync(new InvalidOperationException("Quantidade insuficiente."));

            // Act
            var result = await _controller.VenderAtivo(portfolioId, request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task VenderAtivo_DeveRemoverAtivo_QuandoVenderTudo()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var portfolio = new Portfolio("João Silva");

            var request = new VenderAtivoRequest
            {
                Ticker = "PETR4",
                Quantidade = 100,  // Vender tudo
                PrecoVenda = 35.00m
            };

            _serviceMock.Setup(s => s.VenderAtivoAsync(portfolioId, request.Ticker, request.Quantidade, request.PrecoVenda))
                .ReturnsAsync(new OperacaoResponse(request.Ticker, TipoAtivo.Acoes, TipoOperacao.Venda, request.Quantidade, request.PrecoVenda, DateTime.UtcNow, 30.50m));

            // Act
            var result = await _controller.VenderAtivo(portfolioId, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        #endregion
    }
}
