using InvestidorCarteira.Domain.Enums;

namespace InvestidorCarteira.Domain.Entities
{
    public class Transacao
    {
        public Guid Id { get; private set; }
        public string Ticker { get; private set; }
        public DateTime Data { get; private set; }
        public TipoOperacao TipoOperacao { get; private set; } // Compra ou Venda
        public TipoAtivo TipoAtivo { get; private set; }       // Acao, FII, etc
        
        public decimal Quantidade { get; private set; }
        public decimal PrecoUnitario { get; private set; }     // Por quanto negociou
        public decimal Total => Quantidade * PrecoUnitario;
        
        // VITAL PARA IMPOSTO: Qual era o seu PM quando você vendeu?
        public decimal PrecoMedioNaData { get; private set; } 

        // Construtor privado para o EF Core
        protected Transacao() { }

        public Transacao(string ticker, TipoAtivo tipoAtivo, TipoOperacao operacao, decimal quantidade, decimal precoUnitario, decimal precoMedioAtual)
        {
            Id = Guid.NewGuid();
            Data = DateTime.UtcNow; // Ou passar por parametro se quiser retroativo
            Ticker = ticker.ToUpper();
            TipoAtivo = tipoAtivo;
            TipoOperacao = operacao;
            Quantidade = quantidade;
            PrecoUnitario = precoUnitario;
            PrecoMedioNaData = precoMedioAtual;
        }
    }
}