using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using InvestidorCarteira.API.Controllers;
using InvestidorCarteira.Application.DTOs;
using InvestidorCarteira.Application.Interfaces;
using InvestidorCarteira.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InvestidorCarteira.Tests.Controllers
{
    public class RelatorioControllerTests
    {
        private readonly Mock<IGerarRelatorioImpostoUseCase> _useCaseMock;
        private readonly RelatorioController _controller;

        public RelatorioControllerTests()
        {
            _useCaseMock = new Mock<IGerarRelatorioImpostoUseCase>();
            _controller = new RelatorioController(_useCaseMock.Object);
        }

        #region ObterRelatorioMensal Tests

        [Fact]
        public async Task ObterRelatorioMensal_DeveRetornarOk_QuandoSucessoComOperacoes()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var ano = 2023;
            var mes = 10;

            var resultadoEsperado = new ApuracaoResultadoDto
            {
                Results = new List<OperationResultDto>
                {
                    new OperationResultDto
                    {
                        Operation = new OperationDto
                        {
                            Ticker = "PETR4",
                            Tipo = TipoAtivo.Acoes,
                            Quantidade = 100,
                            PrecoMedioHistorico = 30m,
                            ValorVenda = 3500m,
                            Date = new DateTime(ano, mes, 15),
                            IsDayTrade = false
                        },
                        Imposto = 75m  // 15% de (3500 - 3000)
                    }
                },
                TotalImposto = 75m
            };

            _useCaseMock.Setup(u => u.Executar(portfolioId, ano, mes))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var result = await _controller.ObterRelatorioMensal(portfolioId, ano, mes);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(resultadoEsperado);

            _useCaseMock.Verify(u => u.Executar(portfolioId, ano, mes), Times.Once);
        }

        [Fact]
        public async Task ObterRelatorioMensal_DeveRetornarOk_QuandoMultiplasOperacoes()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var ano = 2023;
            var mes = 11;

            var resultadoEsperado = new ApuracaoResultadoDto
            {
                Results = new List<OperationResultDto>
                {
                    new OperationResultDto
                    {
                        Operation = new OperationDto { Ticker = "PETR4", Tipo = TipoAtivo.Acoes, Quantidade = 100, ValorVenda = 3500m },
                        Imposto = 75m
                    },
                    new OperationResultDto
                    {
                        Operation = new OperationDto { Ticker = "VALE3", Tipo = TipoAtivo.Acoes, Quantidade = 50, ValorVenda = 4000m },
                        Imposto = 150m
                    }
                },
                TotalImposto = 225m
            };

            _useCaseMock.Setup(u => u.Executar(portfolioId, ano, mes))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var result = await _controller.ObterRelatorioMensal(portfolioId, ano, mes);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var dto = okResult.Value as ApuracaoResultadoDto;
            
            dto.Should().NotBeNull();
            dto.Results.Should().HaveCount(2);
            dto.TotalImposto.Should().Be(225m);
        }

        [Fact]
        public async Task ObterRelatorioMensal_DeveRetornarOk_QuandoSemTransacoes()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var ano = 2023;
            var mes = 12;

            var resultadoEsperado = new ApuracaoResultadoDto
            {
                Results = new List<OperationResultDto>(),
                TotalImposto = 0m
            };

            _useCaseMock.Setup(u => u.Executar(portfolioId, ano, mes))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var result = await _controller.ObterRelatorioMensal(portfolioId, ano, mes);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var dto = okResult.Value as ApuracaoResultadoDto;
            
            dto.Should().NotBeNull();
            dto.Results.Should().BeEmpty();
            dto.TotalImposto.Should().Be(0);
        }

        [Fact]
        public async Task ObterRelatorioMensal_DeveRetornarBadRequest_QuandoCarteiraInexistente()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var ano = 2023;
            var mes = 10;

            _useCaseMock.Setup(u => u.Executar(portfolioId, ano, mes))
                .ThrowsAsync(new Exception("Carteira não encontrada"));

            // Act
            var result = await _controller.ObterRelatorioMensal(portfolioId, ano, mes);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Carteira não encontrada");
        }

        [Fact]
        public async Task ObterRelatorioMensal_DeveRetornarBadRequest_QuandoMesInvalido()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var ano = 2023;
            var mes = 13;  // Mês inválido

            _useCaseMock.Setup(u => u.Executar(portfolioId, ano, mes))
                .ThrowsAsync(new ArgumentException("Mês inválido."));

            // Act
            var result = await _controller.ObterRelatorioMensal(portfolioId, ano, mes);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ObterRelatorioMensal_DeveRetornarBadRequest_QuandoAnoInvalido()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var ano = 1900;  // Ano inválido
            var mes = 10;

            _useCaseMock.Setup(u => u.Executar(portfolioId, ano, mes))
                .ThrowsAsync(new ArgumentException("Ano inválido."));

            // Act
            var result = await _controller.ObterRelatorioMensal(portfolioId, ano, mes);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ObterRelatorioMensal_DeveChamarUseCaseComParametrosCorretos()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var ano = 2024;
            var mes = 6;

            var resultadoEsperado = new ApuracaoResultadoDto
            {
                Results = new List<OperationResultDto>
                {
                    new OperationResultDto
                    {
                        Operation = new OperationDto { Ticker = "PETR4", Tipo = TipoAtivo.Acoes },
                        Imposto = 150m
                    }
                },
                TotalImposto = 150m
            };

            _useCaseMock.Setup(u => u.Executar(portfolioId, ano, mes))
                .ReturnsAsync(resultadoEsperado);

            // Act
            await _controller.ObterRelatorioMensal(portfolioId, ano, mes);

            // Assert
            _useCaseMock.Verify(u => u.Executar(
                It.Is<Guid>(id => id == portfolioId),
                It.Is<int>(a => a == ano),
                It.Is<int>(m => m == mes)
            ), Times.Once);
        }

        [Fact]
        public async Task ObterRelatorioMensal_DeveCalcularImpostoCorretamente_QuandoLucroPositivo()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var ano = 2023;
            var mes = 5;

            var resultadoEsperado = new ApuracaoResultadoDto
            {
                Results = new List<OperationResultDto>
                {
                    new OperationResultDto
                    {
                        Operation = new OperationDto
                        {
                            Ticker = "VALE3",
                            Tipo = TipoAtivo.Acoes,
                            Quantidade = 100,
                            PrecoMedioHistorico = 50m,
                            ValorVenda = 8000m,
                            Date = new DateTime(ano, mes, 10)
                        },
                        Imposto = 450m  // 15% de (8000 - 5000)
                    }
                },
                TotalImposto = 450m
            };

            _useCaseMock.Setup(u => u.Executar(portfolioId, ano, mes))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var result = await _controller.ObterRelatorioMensal(portfolioId, ano, mes);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var dto = okResult.Value as ApuracaoResultadoDto;
            
            dto.TotalImposto.Should().Be(450m);
            dto.Results.Should().HaveCount(1);
        }

        [Fact]
        public async Task ObterRelatorioMensal_DeveAceitarDiferentesTiposDeAtivos()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var ano = 2023;
            var mes = 8;

            var resultadoEsperado = new ApuracaoResultadoDto
            {
                Results = new List<OperationResultDto>
                {
                    new OperationResultDto
                    {
                        Operation = new OperationDto { Ticker = "PETR4", Tipo = TipoAtivo.Acoes, Quantidade = 100 },
                        Imposto = 100m
                    },
                    new OperationResultDto
                    {
                        Operation = new OperationDto { Ticker = "HGLG11", Tipo = TipoAtivo.FIIs, Quantidade = 50 },
                        Imposto = 200m
                    },
                    new OperationResultDto
                    {
                        Operation = new OperationDto { Ticker = "BTC", Tipo = TipoAtivo.Criptomoedas, Quantidade = 1 },
                        Imposto = 300m
                    },
                    new OperationResultDto
                    {
                        Operation = new OperationDto { Ticker = "IVVB11", Tipo = TipoAtivo.ETFs, Quantidade = 20 },
                        Imposto = 150m
                    }
                },
                TotalImposto = 750m
            };

            _useCaseMock.Setup(u => u.Executar(portfolioId, ano, mes))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var result = await _controller.ObterRelatorioMensal(portfolioId, ano, mes);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var dto = okResult.Value as ApuracaoResultadoDto;
            
            dto.Results.Should().HaveCount(4);
            dto.Results.Should().Contain(r => r.Operation.Tipo == TipoAtivo.Acoes);
            dto.Results.Should().Contain(r => r.Operation.Tipo == TipoAtivo.FIIs);
            dto.Results.Should().Contain(r => r.Operation.Tipo == TipoAtivo.Criptomoedas);
            dto.Results.Should().Contain(r => r.Operation.Tipo == TipoAtivo.ETFs);
            dto.TotalImposto.Should().Be(750m);
        }

        #endregion
    }
}
