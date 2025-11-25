using InvestidorCarteira.Domain.Entities;
using Xunit; // O framework de testes

namespace InvestidorCarteira.Tests.Domain
{
    public class AcaoTests
    {
        [Fact] // Fact significa: "Isso é um teste único e verdadeiro"
        public void Deve_Isentar_Imposto_Se_Venda_Total_Menor_Que_20k_SwingTrade()
        {
            // 1. ARRANGE (Preparar o cenário)
            // Comprei 100 ações a R$ 10,00 (Total gasto: R$ 1.000)
            var acao = new Acoes("PETR4", 100, 10.00m);
            
            decimal valorVendaUnitario = 20.00m; // Vendi pelo dobro
            decimal valorTotalVenda = 100 * valorVendaUnitario; // R$ 2.000,00
            
            // O usuário vendeu apenas R$ 2.000,00 no mês todo.
            decimal totalVendasNoMes = 2000.00m; 
            bool isDayTrade = false;

            // 2. ACT (Executar a ação)
            var imposto = acao.CalcularImpostoEstimado(valorTotalVenda, totalVendasNoMes, isDayTrade);

            // 3. ASSERT (Validar o resultado)
            // Lucro foi de 1.000, mas como vendeu < 20k, imposto deve ser 0.
            Assert.Equal(0, imposto);
        }

        [Fact]
        public void Deve_Cobrar_15_Porcento_Se_Venda_Total_Maior_Que_20k_SwingTrade()
        {
            // ARRANGE
            var acao = new Acoes("VALE3", 1000, 10.00m); // Custo: 10.000
            
            // Venda Total: 25.000 (Lucro de 15.000)
            decimal valorTotalVenda = 25000.00m; 
            
            // O usuário vendeu > 20k, então perde a isenção
            decimal totalVendasNoMes = 25000.00m; 
            bool isDayTrade = false;

            // ACT
            var imposto = acao.CalcularImpostoEstimado(valorTotalVenda, totalVendasNoMes, isDayTrade);

            // ASSERT
            // Esperado: 15% de 15.000 (Lucro) = 2.250
            Assert.Equal(2250.00m, imposto);
        }

        [Fact]
        public void Deve_Cobrar_20_Porcento_Se_For_DayTrade_Mesmo_Abaixo_De_20k()
        {
            // ARRANGE
            var acao = new Acoes("WEGE3", 100, 10.00m);
            decimal valorTotalVenda = 2000.00m; // Lucro de 1.000
            
            decimal totalVendasNoMes = 2000.00m; // Pouca venda
            bool isDayTrade = true; // MAS É DAY TRADE!

            // ACT
            var imposto = acao.CalcularImpostoEstimado(valorTotalVenda, totalVendasNoMes, isDayTrade);

            // ASSERT
            // Day Trade não tem conversa: 20% do lucro.
            // 20% de 1.000 = 200.
            Assert.Equal(200.00m, imposto);
        }
    }
}