using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimplCommerce.WebHost.Migrations
{
    public partial class AddCustomerIdToOrderAndNotRequireShippingBillingAddresses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ShippingAddressId",
                table: "Orders_Order",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "BillingAddressId",
                table: "Orders_Order",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "CustomerId",
                table: "Orders_Order",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Order_CustomerId",
                table: "Orders_Order",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Order_Core_User_CustomerId",
                table: "Orders_Order",
                column: "CustomerId",
                principalTable: "Core_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Order_Core_User_CustomerId",
                table: "Orders_Order");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Order_CustomerId",
                table: "Orders_Order");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Orders_Order");

            migrationBuilder.AlterColumn<long>(
                name: "ShippingAddressId",
                table: "Orders_Order",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "BillingAddressId",
                table: "Orders_Order",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);
        }
    }
}
