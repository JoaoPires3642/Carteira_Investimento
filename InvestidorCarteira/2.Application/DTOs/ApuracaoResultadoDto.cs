using System.Collections.Generic;
using InvestidorCarteira.Application.DTOs;

namespace InvestidorCarteira.Application.DTOs
{
    public class OperationResultDto
    {
        public OperationDto Operation { get; set; }
        public decimal Imposto { get; set; }
    }

    public class ApuracaoResultadoDto
    {
        public List<OperationResultDto> Results { get; set; } = new List<OperationResultDto>();
        public decimal TotalImposto { get; set; }
    }
}
