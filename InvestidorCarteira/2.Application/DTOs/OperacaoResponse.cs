namespace InvestidorCarteira.Application.DTOs;

using InvestidorCarteira.Domain.Enums;

public record OperacaoResponse(
    string Ticker,
    TipoAtivo TipoAtivo,
    TipoOperacao TipoOperacao,
    decimal Quantidade,
    decimal PrecoUnitario,
    DateTime Data,
    decimal PrecoMedioNaData
);