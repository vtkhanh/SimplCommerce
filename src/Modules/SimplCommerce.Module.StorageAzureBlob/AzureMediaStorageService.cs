using Microsoft.Extensions.Options;
using SimplCommerce.Module.Core.Services;

namespace SimplCommerce.Module.StorageAzureBlob
{
    public class AzureMediaStorageService : AzureBlobStorage, IMediaService
    {
        public AzureMediaStorageService(IOptionsSnapshot<AzureStorageConfig> storageConfig) 
            : base(storageConfig.Get("AzureMediaStorageConfig"))
        {
        }
    }
}
