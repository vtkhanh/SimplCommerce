using Microsoft.EntityFrameworkCore.Migrations;

namespace SimplCommerce.WebHost.Migrations
{
    public partial class AddLastImportResultToOrderFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LastImportResultId",
                table: "Orders_OrderFile",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderFile_LastImportResultId",
                table: "Orders_OrderFile",
                column: "LastImportResultId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderFile_Orders_ImportResult_LastImportResultId",
                table: "Orders_OrderFile",
                column: "LastImportResultId",
                principalTable: "Orders_ImportResult",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderFile_Orders_ImportResult_LastImportResultId",
                table: "Orders_OrderFile");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderFile_LastImportResultId",
                table: "Orders_OrderFile");

            migrationBuilder.DropColumn(
                name: "LastImportResultId",
                table: "Orders_OrderFile");
        }
    }
}
