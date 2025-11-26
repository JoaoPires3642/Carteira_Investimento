using System.ComponentModel.DataAnnotations;

namespace InvestidorCarteira.Infrastructure.DTOs
{
    public class CriarCarteiraRequest
    {
        [Required]
        public string NomeTitular { get; set; } = string.Empty;
    }
}