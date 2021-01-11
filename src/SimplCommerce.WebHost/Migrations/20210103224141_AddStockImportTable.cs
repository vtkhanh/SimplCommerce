using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimplCommerce.WebHost.Migrations
{
    public partial class AddStockImportTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Catalog_StockImport",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTimeOffset>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    SupplierId = table.Column<long>(nullable: false),
                    Cost = table.Column<decimal>(nullable: false),
                    NewPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_StockImport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_StockImport_Catalog_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Catalog_Supplier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_StockImport_SupplierId",
                table: "Catalog_StockImport",
                column: "SupplierId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Catalog_StockImport");
        }
    }
}
