using InvestidorCarteira.Domain.Entities;
using Xunit; // O framework de testes

namespace InvestidorCarteira.Tests.Domain
{
    public class CriptomoedasTests
    {
        [Fact]
        public void Deve_Cobrar_15_Porcento_Se_Venda_Total_Maior_Que_20k_SwingTrade()
        {
            // ARRANGE
            var cripto = new Criptomoedas("BTC", 1, 10000.00m); // Custo: 10.000
            
            // Venda Total: 40.000 (Lucro de 30.000)
            decimal valorTotalVenda = 40000.00m; 
            
            // O usuário vendeu > 35k, então perde a isenção
            decimal totalVendasNoMes = 40000.00m; 
            bool isDayTrade = false;

            // ACT
            var imposto = cripto.CalcularImpostoEstimado(valorTotalVenda, totalVendasNoMes, isDayTrade);

            // ASSERT
            // Esperado: 15% de 30.000 (Lucro) = 4.500
            Assert.Equal(4500.00m, imposto);
        }
        [Fact]
        public void Deve_Isentar_Imposto_Se_Venda_Total_Menor_Que_35k_SwingTrade()
        {
            // ARRANGE
            var cripto = new Criptomoedas("ETH", 5, 2000.00m); // Custo: 10.000     
            // Venda Total: 30.000 (Lucro de 20.000)
            decimal valorTotalVenda = 30000.00m; 
            // O usuário vendeu <= 35k, então tem isenção
            decimal totalVendasNoMes = 30000.00m; 
            bool isDayTrade = false;        
            // ACT
            var imposto = cripto.CalcularImpostoEstimado(valorTotalVenda, totalVendasNoMes, isDayTrade);
            // ASSERT
            // Esperado: isenção total
            Assert.Equal(0.00m, imposto);
        }
        [Fact]
        public void Deve_Isentar_Imposto_Se_Houver_Prejuizo()
        {
            // ARRANGE
            var cripto = new Criptomoedas("XRP", 1000, 5.00m); // Custo: 5.000     
            // Venda Total: 4.000 (Prejuízo de 1.000)
            decimal valorTotalVenda = 4000.00m; 
            // O usuário vendeu qualquer valor, mas teve prejuízo
            decimal totalVendasNoMes = 4000.00m; 
            bool isDayTrade = false;        
            // ACT
            var imposto = cripto.CalcularImpostoEstimado(valorTotalVenda, totalVendasNoMes, isDayTrade);
            // ASSERT
            // Esperado: isenção total por prejuízo
            Assert.Equal(0.00m, imposto);
        }
    }
}