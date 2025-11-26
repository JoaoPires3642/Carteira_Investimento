using InvestidorCarteira.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvestidorCarteira.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        // Construtor: Recebe as opções (ex: "Use SQLite") e passa para a classe pai
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Mapeamento das Tabelas
        // Você não precisa criar DbSet para Acao ou FII separado. 
        // O EF Core entende que eles são filhos de 'Ativos'.
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Ativos> Ativos { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }

        // Configurações detalhadas das tabelas
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Configurando a Herança (TPH - Table Per Hierarchy)
            // Cria uma coluna "TipoAtivo" no banco para saber se é Acao ou FII
            modelBuilder.Entity<Ativos>()
                .HasDiscriminator<string>("TipoAtivo")
                .HasValue<Acoes>("Acao")
                .HasValue<FIIs>("FII")
                .HasValue<ETFs>("ETF")
                .HasValue<Criptomoedas>("Cripto");

            // 2. Configurando Decimais (Dinheiro)
            // O Banco precisa saber quantas casas decimais usar. 
            // (18,2) significa 18 dígitos no total, sendo 2 depois da vírgula.
            // Para Cripto, talvez precisemos de mais casas, vou colocar (18,8) na Quantidade.

            modelBuilder.Entity<Ativos>()
                .Property(a => a.PrecoMedio)
                .HasPrecision(18, 2); // R$ 10.00

            modelBuilder.Entity<Ativos>()
                .Property(a => a.Quantidade)
                .HasPrecision(18, 8); // 0.00000450 BTC
            // 3. Relacionamento: Portfolio tem muitos Ativos
            // Isso garante que se apagar o Portfolio, apaga os ativos dele (Cascade)
            modelBuilder.Entity<Portfolio>()
                .HasMany(p => p.Ativos)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            // 3b. Relacionamento: Portfolio tem muitas Transacoes
            // Permite que o EF rastreie transações adicionadas à coleção e faça INSERTs.
            modelBuilder.Entity<Portfolio>()
                .HasMany(p => p.Transacoes)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

                // Configuração da Transação
    modelBuilder.Entity<Portfolio>()
        .HasMany(p => p.Transacoes) // Portfolio tem muitas Transações
        .WithOne()
        .OnDelete(DeleteBehavior.Cascade);

modelBuilder.Entity<Transacao>()
        .Property(t => t.Id)
        .ValueGeneratedNever();

    modelBuilder.Entity<Transacao>()
        .Property(t => t.PrecoUnitario)
        .HasPrecision(18, 2);

    modelBuilder.Entity<Transacao>()
        .Property(t => t.PrecoMedioNaData)
        .HasPrecision(18, 2);

    modelBuilder.Entity<Transacao>()
        .Property(t => t.Quantidade)
        .HasPrecision(18, 8);
}
        }
        
    }