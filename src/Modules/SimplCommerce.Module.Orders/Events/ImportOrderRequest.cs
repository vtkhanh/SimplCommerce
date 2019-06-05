using MediatR;

namespace SimplCommerce.Module.Orders.Events
{
    internal class ImportOrderRequest : IRequest
    {
        public ImportOrderRequest(long orderFileId, string referenceFileName)
        {
            OrderFileId = orderFileId;
            ReferenceFileName = referenceFileName;
        }

        public long OrderFileId { get; }

        public string ReferenceFileName { get; }
    }
}
