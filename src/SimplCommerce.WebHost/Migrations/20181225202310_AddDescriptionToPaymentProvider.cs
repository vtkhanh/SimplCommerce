using Microsoft.EntityFrameworkCore.Migrations;

namespace SimplCommerce.WebHost.Migrations
{
    public partial class AddDescriptionToPaymentProvider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Payment_Orders_Order_OrderId",
                table: "Payments_Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payments_Payment_OrderId",
                table: "Payments_Payment");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Payments_PaymentProvider",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PaymentProviderId",
                table: "Orders_Order",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Order_PaymentProviderId",
                table: "Orders_Order",
                column: "PaymentProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Order_Payments_PaymentProvider_PaymentProviderId",
                table: "Orders_Order",
                column: "PaymentProviderId",
                principalTable: "Payments_PaymentProvider",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Order_Payments_PaymentProvider_PaymentProviderId",
                table: "Orders_Order");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Order_PaymentProviderId",
                table: "Orders_Order");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Payments_PaymentProvider");

            migrationBuilder.DropColumn(
                name: "PaymentProviderId",
                table: "Orders_Order");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Payment_OrderId",
                table: "Payments_Payment",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Payment_Orders_Order_OrderId",
                table: "Payments_Payment",
                column: "OrderId",
                principalTable: "Orders_Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
