using InvestidorCarteira.Domain.Entities;
using Xunit;

namespace InvestidorCarteira.Tests.Domain
{
    public class FiiTests
    {
        [Fact]
        public void Deve_Cobrar_20_Porcento_Sempre_Mesmo_Com_Venda_Baixa()
        {
            // ARRANGE
            // Comprei 10 FIIs a R$ 100 (Custo 1.000)
            var fii = new FIIs("HGLG11", 10, 100.00m);
            
            // Vendi por R$ 1.500 (Lucro 500)
            decimal valorTotalVenda = 1500.00m;
            
            // Venda ridícula no mês, mas FII não perdoa !_!.
            decimal totalVendasNoMes = 1500.00m; 
            bool isDayTrade = false;

            // ACT
            var imposto = fii.CalcularImpostoEstimado(valorTotalVenda, totalVendasNoMes, isDayTrade);

            // ASSERT
            // 20% de 500 = 100.
            Assert.Equal(100.00m, imposto);
        }
    }
}