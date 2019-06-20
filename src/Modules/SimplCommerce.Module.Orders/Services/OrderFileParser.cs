using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
using Microsoft.EntityFrameworkCore.Internal;
using SimplCommerce.Infrastructure;
using SimplCommerce.Module.Orders.Services.Dtos;
using static SimplCommerce.Module.Orders.Services.Constants.OrderFileConstants;

namespace SimplCommerce.Module.Orders.Services
{
    internal class OrderFileParser : IOrderFileParser
    {
        public IEnumerable<ImportingOrderDto> Parse(Stream fileStream)
        {
            var result = new List<ImportingOrderDto>();

            using (var reader = ExcelReaderFactory.CreateReader(fileStream))
            {
                var sheets = reader.AsDataSet();

                if (!sheets.Tables.Any())
                    return result;

                sheets.Tables[0].Rows.RemoveAt(0); // Remove header row
                foreach (DataRow row in sheets.Tables[0].Rows)
                {
                    var externalOrderId = row[ColumnIndex.ExternalOrderId].ToString();
                    if (externalOrderId.HasValue())
                    {
                        var dto = new ImportingOrderDto
                        {
                            ExternalOrderId = externalOrderId,
                            OrderedDate = DateTime.ParseExact(row[ColumnIndex.OrderedDate].ToString(), FileDateTimeFormat, null),
                            Status = OrderStatusMapping[row[ColumnIndex.Status].ToString()],
                            TrackingNumber = row[ColumnIndex.TrackingNumber].ToString(),
                            Sku = row[ColumnIndex.Sku].ToString(),
                            Price = decimal.Parse(row[ColumnIndex.Price].ToString()),
                            Quantity = int.Parse(row[ColumnIndex.Quantity].ToString()),
                            ShippingCost = decimal.Parse(row[ColumnIndex.ShippingCost].ToString()),
                            ShippingAmount = decimal.Parse(row[ColumnIndex.ShippingAmount].ToString()),
                            OrderTotal = decimal.Parse(row[ColumnIndex.OrderTotal].ToString()),
                            Username = row[ColumnIndex.Username].ToString(),
                            Phone = row[ColumnIndex.Phone].ToString(),
                            ShippingAddress = row[ColumnIndex.ShippingAddress].ToString()
                        };
                        result.Add(dto);
                    }
                }
            }

            return result;
        }
    }
}
