using System.Text.Json.Serialization;
using InvestidorCarteira.Domain.Enums;

namespace InvestidorCarteira.Infrastructure.DTOs
{
    public class ComprarAtivoRequest
    {
        public required string Ticker { get; set; }
        public decimal Quantidade { get; set; }
        public decimal PrecoPago { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required TipoAtivo Tipo { get; set; }
    }
}