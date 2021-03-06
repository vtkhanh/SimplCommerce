﻿using System;

namespace SimplCommerce.Module.Orders.Services.Dtos
{
    public class GetOrderFileDto
    {
        public long Id { get; set; }

        public string FileName { get; set; }

        public string ReferenceFileName { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public string Status { get; set; }

        public long ImportResultId { get; set; }

        public int SuccessCount { get; set; }

        public int FailureCount { get; set; }
    }
}
