using Microsoft.EntityFrameworkCore.Migrations;

namespace SimplCommerce.WebHost.Migrations
{
    public partial class AddSP_UpdateShippingCostByShippingAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = @"IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Orders')
                        BEGIN
                            EXEC('CREATE SCHEMA Orders')
                        END

                        GO

                        -- Create a new stored procedure called 'UpdateShippingCostByShippingAmount' in schema 'Orders'
                        -- Drop the stored procedure if it already exists
                        IF EXISTS (
                            SELECT *
                            FROM INFORMATION_SCHEMA.ROUTINES
                            WHERE SPECIFIC_SCHEMA = N'Orders'
                                AND SPECIFIC_NAME = N'UpdateShippingCostByShippingAmount'
                        )
                        BEGIN
                            DROP PROCEDURE Orders.UpdateShippingCostByShippingAmount
                        END

                        GO

                        -- Create the stored procedure in the specified schema
                        CREATE PROCEDURE Orders.UpdateShippingCostByShippingAmount
                        AS
                            -- Update ShippingCost according to ShippingAmount
                            UPDATE Orders_Order
                            SET ShippingCost = ShippingAmount
                            WHERE ShippingCost = 0

                            -- Update OrderTotalCost
                            SELECT O.Id, SUM(P.Cost * OI.Quantity) + O.ShippingCost AS OrderTotalCost
                            INTO #OrdersWithTotalCost
                            FROM Orders_Order O
                                INNER JOIN Orders_OrderItem OI ON O.Id = OI.OrderId
                                INNER JOIN Catalog_Product P ON OI.ProductId = P.Id
                            GROUP BY O.Id, O.ShippingCost

                            UPDATE O
                            SET O.OrderTotalCost = OC.OrderTotalCost
                            FROM Orders_Order O
                                INNER JOIN #OrdersWithTotalCost OC ON O.Id = OC.Id
                            WHERE O.OrderTotalCost = 0

                        GO
                        ";
            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sql = @"IF EXISTS (
                            SELECT *
                            FROM INFORMATION_SCHEMA.ROUTINES
                            WHERE SPECIFIC_SCHEMA = N'Orders'
                                AND SPECIFIC_NAME = N'UpdateShippingCostByShippingAmount'
                        )
                        BEGIN
                            DROP PROCEDURE Orders.UpdateShippingCostByShippingAmount
                        END

                        GO";

            migrationBuilder.Sql(sql);
        }
    }
}
