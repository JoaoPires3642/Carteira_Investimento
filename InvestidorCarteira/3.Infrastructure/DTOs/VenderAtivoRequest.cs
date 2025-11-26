namespace InvestidorCarteira.Infrastructure.DTOs
{
    public class VenderAtivoRequest
    {
        public required string Ticker { get; set; }
        public decimal Quantidade { get; set; }
        public decimal PrecoVenda { get; set; }
    }
}