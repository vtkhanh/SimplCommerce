using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Imports;
using SimplCommerce.Infrastructure.ResultTypes;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.Models.Enums;
using SimplCommerce.Module.Orders.Services;
using SimplCommerce.Module.Orders.Services.Dtos;

namespace SimplCommerce.Module.Orders.Events
{
    internal class ImportOrderHandler : DataFileImport<IEnumerable<ImportingOrderDto>, ImportResult>, IRequestHandler<ImportOrderRequest>
    {
        private readonly IOrderFileService _orderFileService;
        private readonly IOrderImportService _orderImportService;
        private readonly IOrderFileStorageService _fileStorageService;
        private readonly IOrderFileParser _orderFileParser;
        private readonly IRepository<ImportResult> _importResultRepo;

        private string _referenceFileName;

        public ImportOrderHandler(IOrderFileService orderFileService, 
            IOrderImportService orderImportService,
            IOrderFileStorageService fileStorageService,
            IOrderFileParser orderFileParser,
            IRepository<ImportResult> importResultRepo)
        {
            _orderFileService = orderFileService;
            _orderImportService = orderImportService;
            _fileStorageService = fileStorageService;
            _orderFileParser = orderFileParser;
            _importResultRepo = importResultRepo;
        }

        public async Task<Unit> Handle(ImportOrderRequest request, CancellationToken cancellationToken)
        {
            await _orderFileService.UpdateStatusAsync(request.OrderFileId, ImportFileStatus.InProgress);

            _referenceFileName = request.ReferenceFileName;

            var feedback = await RunImportAsync();

            var importResult = feedback.Result;
            importResult.ImportedById = request.ImportedById;
            importResult.OrderFileId = request.OrderFileId;
            _importResultRepo.Add(importResult);
            await _importResultRepo.SaveChangesAsync();
            
            await _orderFileService.UpdateStatusAsync(request.OrderFileId, ImportFileStatus.Completed);

            return Unit.Value;
        }

        protected override async Task<MemoryStream> RetrieveAsync()
        {
            var fileStream = new MemoryStream();
            await _fileStorageService.DownloadToStreamAsync(_referenceFileName, fileStream);
            return fileStream;
        }

        protected override ActionFeedback<IEnumerable<ImportingOrderDto>> Parse(MemoryStream fileStream)
        {
            return _orderFileParser.Parse(fileStream);
        }

        protected override async Task<ActionFeedback<ImportResult>> ImportAsync(IEnumerable<ImportingOrderDto> data)
        {
            var importResult = await _orderImportService.ImportAsync(data);
            return ActionFeedback<ImportResult>.Succeed(importResult);
        }

    }
}
