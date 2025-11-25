using System;

namespace InvestidorCarteira.Application.DTOs
{
    public enum TipoAtivo
    {
        Acoes,
        FII,
        ETF,
        Cripto
    }

    public class OperationDto
    {
        public DateTime Date { get; set; }
        public TipoAtivo Tipo { get; set; }
        public string Ticker { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoMedio { get; set; }
        public decimal ValorVenda { get; set; }
        public bool IsDayTrade { get; set; }
    }
}
