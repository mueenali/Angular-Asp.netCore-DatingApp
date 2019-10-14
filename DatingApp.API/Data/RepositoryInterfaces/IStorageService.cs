
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DatingApp.API.Data.RepositoryInterfaces
{
    public interface IStorageService
    {
        Task<ICloudBlob> UploadImage(IFormFile imageFile);
        Task<bool> DeleteImage(string publicID);

    }
}