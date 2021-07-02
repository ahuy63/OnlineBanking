using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineBanking.Migrations
{
    public partial class addFKCurrencyInCheque : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Cheques",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Cheques_CurrencyId",
                table: "Cheques",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cheques_Currencies_CurrencyId",
                table: "Cheques",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cheques_Currencies_CurrencyId",
                table: "Cheques");

            migrationBuilder.DropIndex(
                name: "IX_Cheques_CurrencyId",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Cheques");
        }
    }
}
