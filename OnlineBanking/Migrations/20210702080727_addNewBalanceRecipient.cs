using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineBanking.Migrations
{
    public partial class addNewBalanceRecipient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NewBalance",
                table: "Transaction",
                newName: "NewBalanceSender");

            migrationBuilder.AddColumn<double>(
                name: "NewBalanceRecipient",
                table: "Transaction",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewBalanceRecipient",
                table: "Transaction");

            migrationBuilder.RenameColumn(
                name: "NewBalanceSender",
                table: "Transaction",
                newName: "NewBalance");
        }
    }
}
