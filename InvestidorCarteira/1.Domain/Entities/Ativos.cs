using InvestidorCarteira.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

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
        public void RegistrarCompra(decimal novaQuantidade, decimal novoPreco)
        {
            var custoTotalAtual = Quantidade * PrecoMedio;
            var custoNovaCompra = novaQuantidade * novoPreco;
            
            Quantidade += novaQuantidade;
            PrecoMedio = (custoTotalAtual + custoNovaCompra) / Quantidade;
        }

        // Registra uma venda, reduzindo a quantidade. Usa decimal para suportar cripto.
        public void RegistrarVenda(decimal quantidade)
        {
            if (quantidade <= 0) throw new ArgumentException("Quantidade deve ser maior que zero");
            if (quantidade > Quantidade) throw new InvalidOperationException("Quantidade insuficiente para venda");

            Quantidade -= quantidade;
        }
        
    
        // Cada tipo de ativo define sua própria regra de cálculo de imposto.
        // Parâmetros adicionais permitem decidir isenção e alíquotas com base
        // no total de vendas no mês e se a operação foi day trade.
        public abstract decimal CalcularImpostoEstimado(decimal valorVenda, decimal vendaTotalNoMes, bool isDayTrade);

        // Expose o tipo do ativo como enum no código. Não mapeado para o banco
        [NotMapped]
        public TipoAtivo Tipo => this switch
        {
            Acoes => TipoAtivo.Acoes,
            Criptomoedas => TipoAtivo.Criptomoedas,
            ETFs => TipoAtivo.ETFs,
            FIIs => TipoAtivo.FIIs,
            _ => throw new InvalidOperationException("Tipo de ativo desconhecido")
        };
    }

}
    
