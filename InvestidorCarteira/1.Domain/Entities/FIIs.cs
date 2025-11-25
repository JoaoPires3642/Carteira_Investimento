namespace InvestidorCarteira.Domain.Entities;

public class FIIs : Ativos
{
    public FIIs(string ticker, int quantidade, decimal precoPago) : base(ticker, quantidade, precoPago)
    {
    }

    public override decimal CalcularImpostoEstimado(decimal valorVenda, decimal vendaTotalNoMes, bool isDayTrade)
    {
        decimal custoTotal = Quantidade * PrecoMedio;
        decimal lucro = valorVenda - custoTotal;

        if (lucro <= 0) return 0;

        // FIIs 20% sobre o lucro
        return lucro * 0.20m;
    }
}
    


