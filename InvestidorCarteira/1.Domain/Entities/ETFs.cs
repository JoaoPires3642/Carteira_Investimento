namespace InvestidorCarteira.Domain.Entities
{
public class ETFs : Ativos
{
    public ETFs(string ticker, decimal quantidade, decimal precoPago) : base(ticker, quantidade, precoPago)
    {
    }

    public override decimal CalcularImpostoEstimado(decimal valorVenda, decimal vendaTotalNoMes, bool isDayTrade)
    {
        decimal custoTotal = Quantidade * PrecoMedio;
        decimal lucro = valorVenda - custoTotal;

        if (lucro <= 0) return 0;

        // Day trade em ETFs (se aplicável): 20%
        if (isDayTrade) return lucro * 0.20m;

        // Regra padrão: 15% sobre o lucro
        return lucro * 0.15m;
    }
}
}   