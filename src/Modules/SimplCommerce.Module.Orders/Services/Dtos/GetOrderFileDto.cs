﻿using System;

namespace SimplCommerce.Module.Orders.Services.Dtos
{
    internal class GetOrderFileDto
    {
        public long Id { get; set; }

        public string FileName { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public string Status { get; set; }
    }
}
