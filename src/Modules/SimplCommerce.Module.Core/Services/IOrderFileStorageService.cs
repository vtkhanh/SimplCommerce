using System.IO;
using System.Threading.Tasks;

namespace SimplCommerce.Module.Core.Services
{
    public interface IOrderFileStorageService : IMediaService
    {
        Task DownloadToStreamAsync(string fileName, Stream fileStream);
    }
}
