using Microsoft.EntityFrameworkCore.Migrations;

namespace SimplCommerce.WebHost.Migrations
{
    public partial class ChangeToUseAddressInsteadOfUserAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Core_User_Core_UserAddress_DefaultBillingAddressId",
                table: "Core_User");

            migrationBuilder.DropForeignKey(
                name: "FK_Core_User_Core_UserAddress_DefaultShippingAddressId",
                table: "Core_User");

            migrationBuilder.AddForeignKey(
                name: "FK_Core_User_Core_Address_DefaultBillingAddressId",
                table: "Core_User",
                column: "DefaultBillingAddressId",
                principalTable: "Core_Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Core_User_Core_Address_DefaultShippingAddressId",
                table: "Core_User",
                column: "DefaultShippingAddressId",
                principalTable: "Core_Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Core_User_Core_Address_DefaultBillingAddressId",
                table: "Core_User");

            migrationBuilder.DropForeignKey(
                name: "FK_Core_User_Core_Address_DefaultShippingAddressId",
                table: "Core_User");

            migrationBuilder.AddForeignKey(
                name: "FK_Core_User_Core_UserAddress_DefaultBillingAddressId",
                table: "Core_User",
                column: "DefaultBillingAddressId",
                principalTable: "Core_UserAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Core_User_Core_UserAddress_DefaultShippingAddressId",
                table: "Core_User",
                column: "DefaultShippingAddressId",
                principalTable: "Core_UserAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
