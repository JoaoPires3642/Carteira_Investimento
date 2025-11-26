namespace InvestidorCarteira.Application.DTOs;

public record PortfolioDto(string Id, string Nome);

public record AtivoDto(string Ticker, decimal Quantidade, decimal PrecoMedio, string Tipo);

public record TransacaoDto(string Ticker, string TipoAtivo, string TipoOperacao, decimal Quantidade, decimal PrecoUnitario, decimal PrecoMedioNaData, DateTime Data);

public class PortfolioDetailsDto
{
    public required string Id { get; init; }
    public required string Nome { get; init; }
    public List<AtivoDto> Ativos { get; init; } = new();
    public List<TransacaoDto> Transacoes { get; init; } = new();
}
