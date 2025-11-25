using System.Text.Json.Serialization;
using InvestidorCarteira.Domain.Enums;

namespace InvestidorCarteira.API.DTOs
{
    public class ComprarAtivoRequest
    {
        public required string Ticker { get; set; }
        public decimal Quantidade { get; set; }
        public decimal PrecoPago { get; set; }

        // Recebemos o enum como string no JSON (ex.: "Acoes", "Criptomoedas")
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required TipoAtivo Tipo { get; set; }
    }
}