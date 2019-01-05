using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimplCommerce.WebHost.Migrations
{
    public partial class AddCompletedOnToOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CompletedOn",
                table: "Orders_Order",
                nullable: true);

            migrationBuilder.Sql(@"UPDATE dbo.Orders_Order
SET CompletedOn = UpdatedOn
WHERE OrderStatus = 6 -- Complete Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedOn",
                table: "Orders_Order");
        }
    }
}
