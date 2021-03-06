﻿using System;
using System.Collections.Generic;

namespace SimplCommerce.Module.Orders.Services.Dtos
{
    public class ImportResultDto
    {
        public IEnumerable<ImportResultDetailDto> Items { get; set; }

        public DateTimeOffset ImportedAt { get; set; }
    }
}
