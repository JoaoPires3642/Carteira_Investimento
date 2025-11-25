using InvestidorCarteira.Domain.Entities;
using Xunit; // O framework de testes

namespace InvestidorCarteira.Tests.Domain
{
    public class ETFsTests
    {
        [Fact]
        public void Deve_Cobrar_15_Porcento_Se_Venda_Lucrosa()
        {
            // ARRANGE
            var etf = new ETFs("IVVB11", 50, 100.00m); // Custo: 5.000
            
            // Venda Total: 7.500 (Lucro de 2.500)
            decimal valorTotalVenda = 7500.00m; 
            
            // O usuário vendeu qualquer valor, mas teve lucro
            decimal totalVendasNoMes = 7500.00m; 
            bool isDayTrade = false;

            // ACT
            var imposto = etf.CalcularImpostoEstimado(valorTotalVenda, totalVendasNoMes, isDayTrade);

            // ASSERT
            // Esperado: 15% de 2.500 (Lucro) = 375
            Assert.Equal(375.00m, imposto);
        }
        [Fact]
        public void Deve_Isentar_Imposto_Se_Houver_Prejuizo()
        {
            // ARRANGE
            var etf = new ETFs("BOVA11", 30, 150.00m); // Custo: 4.500     
            // Venda Total: 4.000 (Prejuízo de 500)
            decimal valorTotalVenda = 4000.00m; 
            // O usuário vendeu qualquer valor, mas teve prejuízo      
            decimal totalVendasNoMes = 4000.00m;
            bool isDayTrade = false;
            // ACT
            var imposto = etf.CalcularImpostoEstimado(valorTotalVenda, totalVendasNoMes, isDayTrade);
            // ASSERT
            // Esperado: isenção total por prejuízo
            Assert.Equal(0.00m, imposto);
        }
    }   
}