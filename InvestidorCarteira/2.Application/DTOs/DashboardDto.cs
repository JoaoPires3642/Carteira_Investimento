namespace InvestidorCarteira.Application.DTOs
{
    public class DashboardDto
    {
        public string NomeTitular { get; set; }
        public decimal TotalInvestido { get; set; } // Quanto tirei do bolso
        public decimal PatrimonioAtual { get; set; } // Quanto vale agora
        
        // Lucro em R$ (Patrimonio - Investido)
        public decimal LucroPrejuizoReais => PatrimonioAtual - TotalInvestido;
        
        // Lucro em %
        public decimal RentabilidadePorcentagem => 
            TotalInvestido == 0 ? 0 : (LucroPrejuizoReais / TotalInvestido) * 100;

        public List<AtivoDashboardItem> Itens { get; set; } = new();
    }

    public class AtivoDashboardItem
    {
        public string Ticker { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoMedio { get; set; } // Meu custo
        public decimal PrecoAtual { get; set; } // Mercado
        
        public decimal ValorInvestido => Quantidade * PrecoMedio;
        public decimal ValorAtual => Quantidade * PrecoAtual;
        
        public decimal Rentabilidade => 
            ValorInvestido == 0 ? 0 : ((ValorAtual - ValorInvestido) / ValorInvestido) * 100;
            
        public decimal DividendYield { get; set; }
        public decimal VariacaoDoDia { get; set; } // Setinha verde/vermelha
    }
}