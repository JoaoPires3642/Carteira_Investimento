using InvestidorCarteira.Application.DTOs;
using InvestidorCarteira.Domain.Entities;
using InvestidorCarteira.Domain.Enums;

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
            // Converte Quantidade para decimal quando necessário
            decimal quantidadeDecimal = (decimal)op.Quantidade;

            return op.Tipo switch
            {
                TipoAtivo.Acoes => new Acoes(op.Ticker, quantidadeDecimal, op.PrecoMedioHistorico),
                TipoAtivo.FIIs => new FIIs(op.Ticker, quantidadeDecimal, op.PrecoMedioHistorico),
                TipoAtivo.ETFs => new ETFs(op.Ticker, quantidadeDecimal, op.PrecoMedioHistorico),
                TipoAtivo.Criptomoedas => new Criptomoedas(op.Ticker, quantidadeDecimal, op.PrecoMedioHistorico),
                _ => new Acoes(op.Ticker, quantidadeDecimal, op.PrecoMedioHistorico),
            };
        }
    }
}
