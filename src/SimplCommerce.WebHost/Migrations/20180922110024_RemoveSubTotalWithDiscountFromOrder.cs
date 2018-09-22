using Microsoft.EntityFrameworkCore.Migrations;

namespace SimplCommerce.WebHost.Migrations
{
    public partial class RemoveSubTotalWithDiscountFromOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubTotalWithDiscount",
                table: "Orders_Order");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalWithDiscount",
                table: "Orders_Order",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
