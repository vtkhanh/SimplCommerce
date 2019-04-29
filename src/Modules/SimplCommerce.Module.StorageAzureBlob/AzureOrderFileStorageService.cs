using Microsoft.Extensions.Options;
using SimplCommerce.Module.Core.Services;

namespace SimplCommerce.Module.StorageAzureBlob
{
    public class AzureOrderFileStorageService : AzureBlobMediaStorage, IOrderFileStorageService
    {
        public AzureOrderFileStorageService(IOptionsSnapshot<AzureStorageConfig> storageConfig)
            : base(storageConfig.Get("AzureOrderFileStorageConfig"))
        {
        }
    }
}
