using System;
using System.Collections.Generic;
using System.Linq;
using InvestidorCarteira.Domain.Enums;

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
        private readonly List<Transacao> _transacoes = new();
        public IReadOnlyCollection<Transacao> Transacoes => _transacoes.AsReadOnly();

        public Portfolio(string nomeTitular)
        {
            Id = Guid.NewGuid();
            NomeTitular = nomeTitular;
        }

        // Construtor sem parâmetros necessário para o EF Core
        protected Portfolio()
        {
            NomeTitular = string.Empty; // evita warning de propriedade não inicializada
        }

        // Método para adicionar um ativo na carteira
        public void ComprarAtivo(Ativos novoAtivo)
        {
        // 1. Lógica de Posição (Igual antes)
        var ativoExistente = _ativos.FirstOrDefault(a => a.Ticker == novoAtivo.Ticker);
        
        decimal pmAntesDaCompra = ativoExistente?.PrecoMedio ?? 0;

        if (ativoExistente != null)
        {
            ativoExistente.RegistrarCompra(novoAtivo.Quantidade, novoAtivo.PrecoMedio);
        }
        else
        {
            _ativos.Add(novoAtivo);
        }

        // 2. Lógica de Histórico (NOVO!)
        // Na compra, o "PrecoMedioNaData" irrelevante para lucro, mas podemos salvar o preço pago.
        var transacao = new Transacao(
            novoAtivo.Ticker, 
            ObterTipoAtivo(novoAtivo), // Precisamos de um helper ou propriedade no Ativo para pegar o Enum
            TipoOperacao.Compra, 
            novoAtivo.Quantidade,
            novoAtivo.PrecoMedio,      // Na compra, o preço unitário é o preço pago
            pmAntesDaCompra            // PM antes de afetar
        );

        _transacoes.Add(transacao);
    }

    // MÉTODO DE VENDER (Atualizado)
    public void VenderAtivo(string ticker, decimal quantidade, decimal precoVenda)
    {
    var tickerNormalizado = ticker.ToUpper().Trim();

        var ativo = _ativos.FirstOrDefault(a => a.Ticker == tickerNormalizado);
        if (ativo == null) 
        throw new Exception($"Ativo '{ticker}' não encontrado. Você possui: {string.Join(", ", _ativos.Select(a => a.Ticker))}");

        // VITAL: Guardamos o PM *antes* de qualquer coisa (embora na venda o PM não mude, é bom garantir)
        decimal precoMedioNoMomentoDaVenda = ativo.PrecoMedio;

        // 1. Registra no Histórico
        var tipoAtivo = ObterTipoAtivo(ativo);

        var transacao = new Transacao(
            tickerNormalizado,
            tipoAtivo,
            TipoOperacao.Venda,
            quantidade,
            precoVenda,
            precoMedioNoMomentoDaVenda 
        );
        _transacoes.Add(transacao);

        // 2. Atualiza Posição
        ativo.RegistrarVenda(quantidade);
        
        if (ativo.Quantidade == 0)
            _ativos.Remove(ativo);
    }

    // Helper simples para descobrir o enum baseada na classe
    private TipoAtivo ObterTipoAtivo(Ativos ativo)
    {
        if (ativo is Acoes) return TipoAtivo.Acoes;
        if (ativo is FIIs) return TipoAtivo.FIIs;
        if (ativo is Criptomoedas) return TipoAtivo.Criptomoedas;
        if (ativo is ETFs) return TipoAtivo.ETFs;
        throw new InvalidOperationException("Tipo de ativo desconhecido");
    }
}
}
