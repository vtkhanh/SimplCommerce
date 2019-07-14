using MediatR;

namespace SimplCommerce.Module.Orders.Events
{
    internal class ImportOrderRequest : IRequest
    {
        public ImportOrderRequest(long userId, long orderFileId, string referenceFileName)
        {
            ImportedById = userId;
            OrderFileId = orderFileId;
            ReferenceFileName = referenceFileName;
        }

        public long OrderFileId { get; }

        public string ReferenceFileName { get; }

        public long ImportedById { get; set; }
    }
}
