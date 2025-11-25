namespace InvestidorCarteira.Domain.Entities;

public class Acoes : Ativos
{
    public Acoes(string ticker, decimal quantidade, decimal precoPago) : base(ticker, quantidade, precoPago)
    {
    }
    public override decimal CalcularImpostoEstimado(decimal valorVenda, decimal vendaTotalNoMes, bool isDayTrade)
    {
        decimal custoTotal = Quantidade * PrecoMedio;
        decimal lucro = valorVenda - custoTotal;

        if (lucro <= 0) return 0;

        // Day trade: 20% sobre o lucro (sem isenção)
        if (isDayTrade) return lucro * 0.20m;

        // Swing trade: isenção se o total de vendas no mês for <= R$20.000
        if (vendaTotalNoMes <= 20000m) return 0;

        // Caso contrário, 15% sobre o lucro
        return lucro * 0.15m;
    }
}
    


