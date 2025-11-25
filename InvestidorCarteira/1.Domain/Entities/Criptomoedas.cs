namespace InvestidorCarteira.Domain.Entities
{
public class Criptomoedas : Ativos
{
    public Criptomoedas(string ticker, decimal quantidade, decimal precoPago) : base(ticker, quantidade, precoPago)
    {
    }

    // Construtor sem parâmetros para EF Core
    protected Criptomoedas() : base()
    {
    }

    public override decimal CalcularImpostoEstimado(decimal valorVenda, decimal vendaTotalNoMes, bool isDayTrade)
    {
        decimal custoTotal = Quantidade * PrecoMedio;
        decimal lucro = valorVenda - custoTotal;

        if (lucro <= 0) return 0;

        // Isenção se o total de vendas no mês for <= R$35.000
        if (vendaTotalNoMes <= 35000m) return 0;

        // Regra simplificada: 15% sobre o lucro
        return lucro * 0.15m;
    }
}
}