using Microsoft.EntityFrameworkCore.Migrations;

namespace SimplCommerce.WebHost.Migrations
{
    public partial class AddProductIdColumnToStockImportTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "Catalog_StockImport",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_StockImport_ProductId",
                table: "Catalog_StockImport",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Catalog_StockImport_Catalog_Product_ProductId",
                table: "Catalog_StockImport",
                column: "ProductId",
                principalTable: "Catalog_Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Catalog_StockImport_Catalog_Product_ProductId",
                table: "Catalog_StockImport");

            migrationBuilder.DropIndex(
                name: "IX_Catalog_StockImport_ProductId",
                table: "Catalog_StockImport");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Catalog_StockImport");
        }
    }
}
