namespace InvestidorCarteira.Domain.ValueObjects
{
    // "record" cria um objeto imutável automaticamente.
    // É mais leve e seguro para transportar dados.
    public record Cotacao(
        string Ticker, 
        decimal PrecoAtual, 
        decimal PrecoFechamentoAnterior, 
        decimal? DividendYield, // Pode ser nulo (Cripto não tem Yield)
        DateTime? DataUltimoDividendo,          
        DateTime UltimaAtualizacao
    )
    {
        // Regra de Domínio: Propriedade calculada para saber a variação %
        public decimal VariacaoDiaria
        {
            get
            {
                if (PrecoFechamentoAnterior == 0) return 0;
                return ((PrecoAtual - PrecoFechamentoAnterior) / PrecoFechamentoAnterior) * 100;
            }
        }
    }
}