using System;
using System.Threading.Tasks;
using InvestidorCarteira.Domain.Interfaces;
using InvestidorCarteira.Domain.ValueObjects;
using YahooFinanceApi;

namespace InvestidorCarteira.Infrastructure.Services
{
    public class YahooFinanceService : IMarketDataService
    {
        public async Task<Cotacao?> ObterCotacaoAsync(string ticker)
        {
            try
            {
                var tickerFormatado = ticker.ToUpper().EndsWith(".SA") ? ticker.ToUpper() : $"{ticker.ToUpper()}.SA";

                var cotacoes = await Yahoo.Symbols(tickerFormatado)
                    .Fields(
                        Field.RegularMarketPrice, 
                        Field.RegularMarketPreviousClose,
                        Field.TrailingAnnualDividendYield
                        // Field.DividendDate Retorna um long (Unix Timestamp)
                    )
                    .QueryAsync();

                if (!cotacoes.ContainsKey(tickerFormatado)) return null;

                var dados = cotacoes[tickerFormatado];

                /*
DateTime? dataDividendo = null;
if (dados.DividendDate > 0)
{
    dataDividendo = DateTimeOffset.FromUnixTimeSeconds(dados.DividendDate).DateTime;
}
*/

                return new Cotacao(
                    Ticker: ticker.ToUpper(),
                    PrecoAtual: (decimal)dados.RegularMarketPrice,
                    PrecoFechamentoAnterior: (decimal)dados.RegularMarketPreviousClose,
                    DividendYield: dados.TrailingAnnualDividendYield > 0 ? (decimal)dados.TrailingAnnualDividendYield * 100 : 0,
                    DataUltimoDividendo: null,
                    UltimaAtualizacao: DateTime.UtcNow
                );
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}