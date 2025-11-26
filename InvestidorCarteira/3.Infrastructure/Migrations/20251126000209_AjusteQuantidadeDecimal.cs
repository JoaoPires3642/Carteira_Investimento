using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestidorCarteira.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjusteQuantidadeDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Quantidade",
                table: "Transacoes",
                type: "TEXT",
                precision: 18,
                scale: 8,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Quantidade",
                table: "Transacoes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldPrecision: 18,
                oldScale: 8);
        }
    }
}
