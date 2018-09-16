-- Update ShippingCosst according to ShippingAmount
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

DROP TABLE #OrdersWithTotalCost
