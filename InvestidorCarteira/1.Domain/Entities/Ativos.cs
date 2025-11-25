namespace InvestidorCarteira.Domain.Entities
{

public abstract class Ativos{
    // Em Clean Arch, entidades têm ID para persistência
    public Guid Id { get; private set; }
    public string Ticker { get; private set; }
    public decimal Quantidade { get; private set;}
    public decimal PrecoMedio { get; private set; }

   protected Ativos(string ticker, decimal quantidade, decimal precoPago)
        {
            if (string.IsNullOrWhiteSpace(ticker))
                throw new ArgumentException("Ticker não pode ser vazio");

            if (quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero");

            Id = Guid.NewGuid(); // Gera ID único automático
            Ticker = ticker.ToUpper();
            Quantidade = quantidade;
            PrecoMedio = precoPago;
        }

        // Construtor sem parâmetros necessário para o EF Core materializar a entidade.
        protected Ativos()
        {
        }

        // Regra de Negócio: Comprar mais muda o preço médio
        public void RegistrarCompra(int novaQuantidade, decimal novoPreco)
        {
            var custoTotalAtual = Quantidade * PrecoMedio;
            var custoNovaCompra = novaQuantidade * novoPreco;
            
            Quantidade += novaQuantidade;
            PrecoMedio = (custoTotalAtual + custoNovaCompra) / Quantidade;
        }
    
        // Cada tipo de ativo define sua própria regra de cálculo de imposto.
        // Parâmetros adicionais permitem decidir isenção e alíquotas com base
        // no total de vendas no mês e se a operação foi day trade.
        public abstract decimal CalcularImpostoEstimado(decimal valorVenda, decimal vendaTotalNoMes, bool isDayTrade);
    }
}
    
