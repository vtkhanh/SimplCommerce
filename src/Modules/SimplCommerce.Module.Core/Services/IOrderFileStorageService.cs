using System.IO;
using System.Threading.Tasks;

namespace SimplCommerce.Module.Core.Services
{
    public interface IOrderFileStorageService
    {
        Task DownloadToStreamAsync(string fileName, Stream fileStream);

        Task SaveMediaAsync(Stream mediaBinaryStream, string fileName, string mimeType = null);
    }
}
