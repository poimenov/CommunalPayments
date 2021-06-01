using Microsoft.EntityFrameworkCore.Migrations;

namespace CommunalPayments.DataAccess.Migrations
{
    public partial class AddBblAndOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bbl",
                table: "Payments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Options",
                table: "PaymentItems",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bbl",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Options",
                table: "PaymentItems");
        }
    }
}
