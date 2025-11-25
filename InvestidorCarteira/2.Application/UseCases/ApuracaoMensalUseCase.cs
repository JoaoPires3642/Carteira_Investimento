using InvestidorCarteira.Application.DTOs;
using InvestidorCarteira.Domain.Entities;

namespace InvestidorCarteira.Application.UseCases
{
    public class ApuracaoMensalUseCase
    {
        // Apura imposto para um mês/ano com base nas operações.
        public ApuracaoResultadoDto Apurar(List<OperationDto> operations, int year, int month)
        {
            var resultado = new ApuracaoResultadoDto();

            var opsNoMes = operations.Where(o => o.Date.Year == year && o.Date.Month == month).ToList();

            // Soma de vendas por tipo de ativo no mês (necessária para isenções)
            var vendaTotalPorTipo = opsNoMes
                .GroupBy(o => o.Tipo)
                .ToDictionary(g => g.Key, g => g.Sum(op => op.ValorVenda));

            foreach (var op in opsNoMes)
            {
                decimal vendaTotalNoMes = vendaTotalPorTipo.ContainsKey(op.Tipo) ? vendaTotalPorTipo[op.Tipo] : 0m;

                Ativos ativo = CriarAtivoParaOperacao(op);

                decimal imposto = ativo.CalcularImpostoEstimado(op.ValorVenda, vendaTotalNoMes, op.IsDayTrade);

                resultado.Results.Add(new OperationResultDto { Operation = op, Imposto = imposto });
                resultado.TotalImposto += imposto;
            }

            return resultado;
        }

        private Ativos CriarAtivoParaOperacao(OperationDto op)
        {
            return op.Tipo switch
            {
                TipoAtivo.Acoes => new Acoes(op.Ticker, op.Quantidade, op.PrecoMedioHistorico),
                TipoAtivo.FII => new FIIs(op.Ticker, op.Quantidade, op.PrecoMedioHistorico),
                TipoAtivo.ETF => new ETFs(op.Ticker, op.Quantidade, op.PrecoMedioHistorico),
                TipoAtivo.Cripto => new Criptomoedas(op.Ticker, op.Quantidade, op.PrecoMedioHistorico),
                _ => new Acoes(op.Ticker, op.Quantidade, op.PrecoMedioHistorico),
            };
        }
    }
}
