using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using InvestidorCarteira.API.Controllers;
using InvestidorCarteira.API.DTOs;
using InvestidorCarteira.Domain.Entities;
using InvestidorCarteira.Domain.Enums;
using InvestidorCarteira.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InvestidorCarteira.Tests.Controllers
{
    public class PortfolioControllerTests
    {
        private readonly Mock<IPortfolioRepository> _repositoryMock;
        private readonly PortfolioController _controller;

        public PortfolioControllerTests()
        {
            _repositoryMock = new Mock<IPortfolioRepository>();
            _controller = new PortfolioController(_repositoryMock.Object);
        }

        #region ObterPorId Tests

        [Fact]
        public async Task ObterPorId_DeveRetornarOk_QuandoCarteiraExiste()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var portfolio = new Portfolio("João Silva");
            _repositoryMock.Setup(r => r.ObterPorIdAsync(portfolioId))
                .ReturnsAsync(portfolio);

            // Act
            var result = await _controller.ObterPorId(portfolioId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(portfolio);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarNotFound_QuandoCarteiraNaoExiste()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.ObterPorIdAsync(portfolioId))
                .ReturnsAsync((Portfolio)null);

            // Act
            var result = await _controller.ObterPorId(portfolioId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Value.Should().Be("Carteira não encontrada.");
        }

        #endregion

        #region CriarCarteira Tests

        [Fact]
        public async Task CriarCarteira_DeveRetornarCreated_QuandoNomeValido()
        {
            // Arrange
            var nomeTitular = "Maria Santos";
            _repositoryMock.Setup(r => r.CriarAsync(It.IsAny<Portfolio>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CriarCarteira(nomeTitular);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult.ActionName.Should().Be(nameof(PortfolioController.ObterPorId));
            
            var portfolio = createdResult.Value as Portfolio;
            portfolio.Should().NotBeNull();
            portfolio.NomeTitular.Should().Be(nomeTitular);
            
            _repositoryMock.Verify(r => r.CriarAsync(It.IsAny<Portfolio>()), Times.Once);
        }

        [Fact]
        public async Task CriarCarteira_DeveRetornarBadRequest_QuandoNomeVazio()
        {
            // Arrange
            var nomeTitular = string.Empty;

            // Act
            var result = await _controller.CriarCarteira(nomeTitular);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("O nome do titular é obrigatório.");
            
            _repositoryMock.Verify(r => r.CriarAsync(It.IsAny<Portfolio>()), Times.Never);
        }

        [Fact]
        public async Task CriarCarteira_DeveRetornarBadRequest_QuandoNomeNulo()
        {
            // Arrange
            string nomeTitular = null;

            // Act
            var result = await _controller.CriarCarteira(nomeTitular);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _repositoryMock.Verify(r => r.CriarAsync(It.IsAny<Portfolio>()), Times.Never);
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

            _repositoryMock.Setup(r => r.ObterPorIdAsync(portfolioId))
                .ReturnsAsync(portfolio);
            _repositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<Portfolio>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ComprarAtivo(portfolioId, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            
            portfolio.Ativos.Should().HaveCount(1);
            portfolio.Ativos.First().Should().BeOfType<Acoes>();
            portfolio.Ativos.First().Ticker.Should().Be("PETR4");
            
            _repositoryMock.Verify(r => r.AtualizarAsync(portfolio), Times.Once);
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

            _repositoryMock.Setup(r => r.ObterPorIdAsync(portfolioId))
                .ReturnsAsync(portfolio);
            _repositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<Portfolio>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ComprarAtivo(portfolioId, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            portfolio.Ativos.Should().HaveCount(1);
            portfolio.Ativos.First().Should().BeOfType<FIIs>();
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

            _repositoryMock.Setup(r => r.ObterPorIdAsync(portfolioId))
                .ReturnsAsync(portfolio);
            _repositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<Portfolio>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ComprarAtivo(portfolioId, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            portfolio.Ativos.Should().HaveCount(1);
            portfolio.Ativos.First().Should().BeOfType<Criptomoedas>();
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

            _repositoryMock.Setup(r => r.ObterPorIdAsync(portfolioId))
                .ReturnsAsync(portfolio);
            _repositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<Portfolio>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ComprarAtivo(portfolioId, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            portfolio.Ativos.Should().HaveCount(1);
            portfolio.Ativos.First().Should().BeOfType<ETFs>();
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

            _repositoryMock.Setup(r => r.ObterPorIdAsync(portfolioId))
                .ReturnsAsync((Portfolio)null);

            // Act
            var result = await _controller.ComprarAtivo(portfolioId, request);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Value.Should().Be("Carteira não encontrada.");
            
            _repositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Portfolio>()), Times.Never);
        }

        #endregion

        #region VenderAtivo Tests

        [Fact]
        public async Task VenderAtivo_DeveRetornarOk_QuandoVendaValida()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var portfolio = new Portfolio("João Silva");
            var acao = new Acoes("PETR4", 100, 30.50m);
            portfolio.ComprarAtivo(acao);

            var request = new VenderAtivoRequest
            {
                Ticker = "PETR4",
                Quantidade = 50,
                PrecoVenda = 35.00m
            };

            _repositoryMock.Setup(r => r.ObterPorIdAsync(portfolioId))
                .ReturnsAsync(portfolio);
            _repositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<Portfolio>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.VenderAtivo(portfolioId, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            portfolio.Ativos.First().Quantidade.Should().Be(50);
            
            _repositoryMock.Verify(r => r.AtualizarAsync(portfolio), Times.Once);
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

            _repositoryMock.Setup(r => r.ObterPorIdAsync(portfolioId))
                .ReturnsAsync((Portfolio)null);

            // Act
            var result = await _controller.VenderAtivo(portfolioId, request);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            _repositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Portfolio>()), Times.Never);
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

            _repositoryMock.Setup(r => r.ObterPorIdAsync(portfolioId))
                .ReturnsAsync(portfolio);

            // Act & Assert
            var result = await _controller.VenderAtivo(portfolioId, request);
            
            result.Should().BeOfType<BadRequestObjectResult>();
            _repositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Portfolio>()), Times.Never);
        }

        [Fact]
        public async Task VenderAtivo_DeveRetornarBadRequest_QuandoQuantidadeInsuficiente()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var portfolio = new Portfolio("João Silva");
            var acao = new Acoes("PETR4", 100, 30.50m);
            portfolio.ComprarAtivo(acao);

            var request = new VenderAtivoRequest
            {
                Ticker = "PETR4",
                Quantidade = 150,  // Maior que a quantidade disponível
                PrecoVenda = 35.00m
            };

            _repositoryMock.Setup(r => r.ObterPorIdAsync(portfolioId))
                .ReturnsAsync(portfolio);

            // Act
            var result = await _controller.VenderAtivo(portfolioId, request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _repositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Portfolio>()), Times.Never);
        }

        [Fact]
        public async Task VenderAtivo_DeveRemoverAtivo_QuandoVenderTudo()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var portfolio = new Portfolio("João Silva");
            var acao = new Acoes("PETR4", 100, 30.50m);
            portfolio.ComprarAtivo(acao);

            var request = new VenderAtivoRequest
            {
                Ticker = "PETR4",
                Quantidade = 100,  // Vender tudo
                PrecoVenda = 35.00m
            };

            _repositoryMock.Setup(r => r.ObterPorIdAsync(portfolioId))
                .ReturnsAsync(portfolio);
            _repositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<Portfolio>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.VenderAtivo(portfolioId, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            portfolio.Ativos.Should().BeEmpty();
            
            _repositoryMock.Verify(r => r.AtualizarAsync(portfolio), Times.Once);
        }

        #endregion
    }
}
