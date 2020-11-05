using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using SimplCommerce.Infrastructure;
using SimplCommerce.Module.Core.Models;

namespace SimplCommerce.Module.StorageAzureBlob
{
    public abstract class AzureBlobStorage
    {
        protected readonly CloudBlobContainer _blobContainer;

        protected AzureBlobStorage(AzureStorageConfig storageConfig)
        {
            if (storageConfig.AccountKey.IsNullOrEmpty() || storageConfig.AccountName.IsNullOrEmpty())
                throw new ArgumentException("Sorry, can't retrieve your azure storage credential from setting");

            if (storageConfig.Container.IsNullOrEmpty())
                throw new ArgumentException("Please provide a name for your container in the azure blob storage");

            // Create storagecredentials object by reading the values from the configuration (appsettings.json)
            StorageCredentials storageCredentials = new StorageCredentials(storageConfig.AccountName, storageConfig.AccountKey);

            // Create cloudstorage account by passing the storagecredentials
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get reference to the blob container by passing the name by reading the value from the configuration (appsettings.json)
            _blobContainer = blobClient.GetContainerReference(storageConfig.Container);
        }

        public async Task DeleteMediaAsync(Media media)
        {
            if (media == null) await Task.CompletedTask;

            await DeleteMediaAsync(media.FileName);
        }

        public async Task DeleteMediaAsync(string fileName)
        {
            // Get the reference to the block blob from the container
            CloudBlockBlob blockBlob = _blobContainer.GetBlockBlobReference(fileName);

            await blockBlob.DeleteIfExistsAsync();
        }

        public string GetMediaUrl(Media media)
        {
            if (media == null) return null;

            return GetMediaUrl(media.FileName);
        }

        public string GetMediaUrl(string fileName)
        {
            CloudBlockBlob blockBlob = _blobContainer.GetBlockBlobReference(fileName);

            return blockBlob?.Uri.AbsoluteUri;
        }

        public string GetThumbnailUrl(Media media)
        {
            if (media == null) return null;

            return GetMediaUrl(media.FileName);
        }

        public async Task SaveMediaAsync(Stream fileStream, string fileName, string mimeType = null)
        {
            // Get the reference to the block blob from the container
            CloudBlockBlob blockBlob = _blobContainer.GetBlockBlobReference(fileName);

            // Upload the file
            await blockBlob.UploadFromStreamAsync(fileStream);
        }

    }
}
