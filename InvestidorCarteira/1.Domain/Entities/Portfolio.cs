namespace InvestidorCarteira.Domain.Entities
{
    public class Portfolio
    {
        public Guid Id { get; private set; }
        public string NomeTitular { get; private set; }
        
        // A lista de ativos que essa carteira possui
        // Usamos 'private set' para obrigar o uso dos métodos Adicionar/Remover
        private readonly List<Ativos> _ativos = new();
        public IReadOnlyCollection<Ativos> Ativos => _ativos.AsReadOnly();

        public Portfolio(string nomeTitular)
        {
            Id = Guid.NewGuid();
            NomeTitular = nomeTitular;
        }

        // Construtor sem parâmetros necessário para o EF Core
        protected Portfolio()
        {
        }

        // Método para adicionar um ativo na carteira
        public void AdicionarAtivo(Ativos ativo)
        {
            // Aqui colocar regras, ex: validação de saldo
            _ativos.Add(ativo);
        }
        
        
        public decimal CalcularPatrimonioTotal()
        {
            // Usa LINQ para somar (Quantidade * PrecoMedio) de todos os ativos
            return System.Linq.Enumerable.Sum(_ativos, a => a.Quantidade * a.PrecoMedio);
        }
    }
}
