using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.Orders.Models.Enums;
using SimplCommerce.Module.Orders.Services;

namespace SimplCommerce.Module.Orders.Events
{
    internal class ImportOrderHandler : IRequestHandler<ImportOrderRequest>
    {
        private readonly IOrderFileService _orderFileService;
        private readonly IOrderImportService _orderImportService;
        private readonly IOrderFileStorageService _fileStorageService;
        private readonly IOrderFileParser _orderFileParser;

        public ImportOrderHandler(IOrderFileService orderFileService, 
            IOrderImportService orderImportService,
            IOrderFileStorageService fileStorageService,
            IOrderFileParser orderFileParser)
        {
            _orderFileService = orderFileService;
            _orderImportService = orderImportService;
            _fileStorageService = fileStorageService;
            _orderFileParser = orderFileParser;
        }

        public async Task<Unit> Handle(ImportOrderRequest request, CancellationToken cancellationToken)
        {
            await _orderFileService.UpdateStatusAsync(request.OrderFileId, ImportFileStatus.InProgress);

            using (var fileStream = new MemoryStream())
            {
                await _fileStorageService.DownloadToStreamAsync(request.ReferenceFileName, fileStream);

                var feedback = _orderFileParser.Parse(fileStream);

                if (feedback.Success)
                    await _orderImportService.ImportAsync(request.OrderFileId, feedback.Result);
            }

            await _orderFileService.UpdateStatusAsync(request.OrderFileId, ImportFileStatus.Completed);

            return Unit.Value;
        }
    }
}
