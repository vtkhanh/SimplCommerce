using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Blob;
using SimplCommerce.Module.Core.Services;

namespace SimplCommerce.Module.StorageAzureBlob
{
    public class AzureOrderFileStorageService : AzureBlobMediaStorage, IOrderFileStorageService
    {
        public AzureOrderFileStorageService(IOptionsSnapshot<AzureStorageConfig> storageConfig)
            : base(storageConfig.Get("AzureOrderFileStorageConfig"))
        {
        }

        public async Task DownloadToStreamAsync(string fileName, Stream fileStream)
        {
            CloudBlockBlob blockBlob = _blobContainer.GetBlockBlobReference(fileName);
            await blockBlob.DownloadToStreamAsync(fileStream);
        }
    }
}
