namespace InvestidorCarteira.API.DTOs
{
    public class VenderAtivoRequest
    {
        public required string Ticker { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoVenda { get; set; }
    }
}
