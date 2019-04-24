using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace SimplCommerce.Infrastructure.Helpers
{
    public static class FormFileHelper
    {
        public static string GetReferenceFileName(this IFormFile file, Guid uid)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Value.Trim('"');
            var referenceFileName = $"{uid}{Path.GetExtension(originalFileName)}";
            return referenceFileName;
        }
    }
}
