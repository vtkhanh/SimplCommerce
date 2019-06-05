using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SimplCommerce.Module.Orders.Models.Enums;
using SimplCommerce.Module.Orders.Services;

namespace SimplCommerce.Module.Orders.Events
{
    internal class ImportOrderHandler : IRequestHandler<ImportOrderRequest>
    {
        private readonly IOrderFileService _orderFileService;

        public ImportOrderHandler(IOrderFileService orderFileService)
        {
            _orderFileService = orderFileService;
        }

        public async Task<Unit> Handle(ImportOrderRequest request, CancellationToken cancellationToken)
        {
            await _orderFileService.UpdateStatusAsync(request.OrderFileId, ImportFileStatus.InProgress);

            return Unit.Value;
        }
    }
}
