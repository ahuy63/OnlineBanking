using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineBanking.Migrations
{
    public partial class addCurrencyForTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Transaction",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_CurrencyId",
                table: "Transaction",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Currencies_CurrencyId",
                table: "Transaction",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Currencies_CurrencyId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_CurrencyId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Transaction");
        }
    }
}
