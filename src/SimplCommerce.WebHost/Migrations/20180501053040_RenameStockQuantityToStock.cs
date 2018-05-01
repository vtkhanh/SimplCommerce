using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimplCommerce.WebHost.Migrations
{
    public partial class RenameStockQuantityToStock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "Catalog_Product");

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Catalog_Product",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Catalog_Product");

            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "Catalog_Product",
                nullable: true);
        }
    }
}
