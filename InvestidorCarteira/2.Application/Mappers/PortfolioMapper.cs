using InvestidorCarteira.Domain.Entities;
using InvestidorCarteira.Application.DTOs;
using InvestidorCarteira.Domain.Enums;

namespace InvestidorCarteira.Application.Mappers;

public static class PortfolioMapper
{
    // Converte entidade `Portfolio` para `PortfolioDto`.
    public static PortfolioDto ToDto(Portfolio entity)
        => new(entity.Id.ToString(), entity.NomeTitular);

    public static PortfolioDetailsDto ToDetailsDto(Portfolio entity)
    {
        var detalhes = new PortfolioDetailsDto
        {
            Id = entity.Id.ToString(),
            Nome = entity.NomeTitular,
            Ativos = entity.Ativos.Select(a => new AtivoDto(
                a.Ticker,
                a.Quantidade,
                a.PrecoMedio,
                ObterTipoAtivoString(a)
            )).ToList(),
            Transacoes = entity.Transacoes.Select(t => new TransacaoDto(
                t.Ticker,
                t.TipoAtivo.ToString(),
                t.TipoOperacao.ToString(),
                t.Quantidade,
                t.PrecoUnitario,
                t.PrecoMedioNaData,
                t.Data
            )).ToList()
        };

        return detalhes;
    }

    private static string ObterTipoAtivoString(Ativos ativo)
    {
        if (ativo is Acoes) return TipoAtivo.Acoes.ToString();
        if (ativo is FIIs) return TipoAtivo.FIIs.ToString();
        if (ativo is Criptomoedas) return TipoAtivo.Criptomoedas.ToString();
        return TipoAtivo.ETFs.ToString();
    }
}