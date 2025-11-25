using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestidorCarteira.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjusteTabelaTransacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacao_Portfolios_PortfolioId",
                table: "Transacao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transacao",
                table: "Transacao");

            migrationBuilder.RenameTable(
                name: "Transacao",
                newName: "Transacoes");

            migrationBuilder.RenameIndex(
                name: "IX_Transacao_PortfolioId",
                table: "Transacoes",
                newName: "IX_Transacoes_PortfolioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transacoes",
                table: "Transacoes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_Portfolios_PortfolioId",
                table: "Transacoes",
                column: "PortfolioId",
                principalTable: "Portfolios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_Portfolios_PortfolioId",
                table: "Transacoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transacoes",
                table: "Transacoes");

            migrationBuilder.RenameTable(
                name: "Transacoes",
                newName: "Transacao");

            migrationBuilder.RenameIndex(
                name: "IX_Transacoes_PortfolioId",
                table: "Transacao",
                newName: "IX_Transacao_PortfolioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transacao",
                table: "Transacao",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transacao_Portfolios_PortfolioId",
                table: "Transacao",
                column: "PortfolioId",
                principalTable: "Portfolios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
