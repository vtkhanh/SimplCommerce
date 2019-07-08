using Microsoft.EntityFrameworkCore.Migrations;

namespace SimplCommerce.WebHost.Migrations
{
    public partial class AddExternalIdToImportResultDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LineNumber",
                table: "Orders_ImportResultDetail");

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Orders_ImportResultDetail",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Orders_ImportResultDetail");

            migrationBuilder.AddColumn<int>(
                name: "LineNumber",
                table: "Orders_ImportResultDetail",
                nullable: false,
                defaultValue: 0);
        }
    }
}
