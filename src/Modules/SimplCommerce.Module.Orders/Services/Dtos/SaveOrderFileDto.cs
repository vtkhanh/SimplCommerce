﻿using System;

namespace SimplCommerce.Module.Orders.Services.Dtos
{
    internal class SaveOrderFileDto
    {
        public string FileName { get; set; }

        public string ReferenceFileName { get; set; }

        public long CreatedById { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }
}
