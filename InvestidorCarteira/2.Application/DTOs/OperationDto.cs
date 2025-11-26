using System;
using InvestidorCarteira.Domain.Enums;

namespace InvestidorCarteira.Application.DTOs
{
    public class OperationDto
    {
        public DateTime Date { get; set; }
        public TipoAtivo Tipo { get; set; }
        public string Ticker { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        // Preço médio histórico da posição do usuário no momento da venda
        // (não é o preço da venda).
        public decimal PrecoMedioHistorico { get; set; }
        public decimal ValorVenda { get; set; }
        public bool IsDayTrade { get; set; }
    }
}
