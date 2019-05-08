using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimplCommerce.WebHost.Migrations
{
    public partial class AddImportOrderTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders_OrderFile",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FileName = table.Column<string>(nullable: true),
                    ReferenceFileName = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreatedById = table.Column<long>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders_OrderFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_OrderFile_Core_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders_ImportResult",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderFileId = table.Column<long>(nullable: false),
                    SuccessCount = table.Column<int>(nullable: false),
                    FailureCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders_ImportResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_ImportResult_Orders_OrderFile_OrderFileId",
                        column: x => x.OrderFileId,
                        principalTable: "Orders_OrderFile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders_ImportResultDetail",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ImportResultId = table.Column<long>(nullable: false),
                    LineNumber = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders_ImportResultDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_ImportResultDetail_Orders_ImportResult_ImportResultId",
                        column: x => x.ImportResultId,
                        principalTable: "Orders_ImportResult",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ImportResult_OrderFileId",
                table: "Orders_ImportResult",
                column: "OrderFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ImportResultDetail_ImportResultId",
                table: "Orders_ImportResultDetail",
                column: "ImportResultId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderFile_CreatedById",
                table: "Orders_OrderFile",
                column: "CreatedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders_ImportResultDetail");

            migrationBuilder.DropTable(
                name: "Orders_ImportResult");

            migrationBuilder.DropTable(
                name: "Orders_OrderFile");
        }
    }
}
