using Microsoft.EntityFrameworkCore.Migrations;

namespace SimplCommerce.WebHost.Migrations
{
    public partial class AddTriggerOnUpdatingOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR ALTER TRIGGER Orders_Order_AfterUpdate   
ON dbo.Orders_Order
AFTER UPDATE   
AS 
BEGIN
    UPDATE dbo.Orders_Order
    SET UpdatedOn = GETDATE()
    FROM dbo.Orders_Order O
        INNER JOIN inserted I ON O.Id = I.Id;  -- Ensure only trigger on affected rows
END;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TRIGGER Orders_Order_AfterUpdate");
        }
    }
}
