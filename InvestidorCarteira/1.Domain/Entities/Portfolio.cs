namespace InvestidorCarteira.Domain.Entities;
public class Portfolio
{
    public string Id { get; }
    public string Nome { get; }
    public Portfolio(string id, string nome)
    {
        Id = id;
        Nome = nome;
    }
}
