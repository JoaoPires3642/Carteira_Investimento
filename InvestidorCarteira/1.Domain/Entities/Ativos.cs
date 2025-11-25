namespace InvestidorCarteira.Domain.Entities;

public class Ativos
{
    public string Id { get; }
    public string Nome { get; }
    public decimal Quantidade { get; }
    public decimal Preco { get; }
    public Ativos(string id, string nome, decimal quantidade, decimal preco)
    {
        Id = id;
        Nome = nome;
        Quantidade = quantidade;
        Preco = preco;
    }
}