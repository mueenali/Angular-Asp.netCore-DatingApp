using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using DatingApp.API.Data.RepositoryInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DatingApp.API.Data
{
    public class StorageService : IStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly string _blobStorageConnectionString;
        private readonly string _imageRootPath;
        private readonly string _imageContainers;
        private readonly CloudStorageAccount storageAccount;
        public StorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            _blobStorageConnectionString = _configuration.GetConnectionString("BlobStorage");
            _imageRootPath = _configuration.GetSection("imageRootPath").Value;
            _imageContainers = _configuration.GetSection("imageContainers").Value;
            CloudStorageAccount.TryParse(_blobStorageConnectionString, out storageAccount);
        }

        public async Task<ICloudBlob> UploadImage(IFormFile imageFile)
        {
            if (storageAccount != null)
            {
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(_imageContainers);
                await container.CreateIfNotExistsAsync();
                var blob = container.GetBlockBlobReference($"{Guid.NewGuid().ToString()}{Path.GetExtension(imageFile.FileName)}");
                await blob.UploadFromStreamAsync(imageFile.OpenReadStream());
                return blob;
            }

            return null;
        }

        public async Task<bool> DeleteImage(string publicID)
        {
            if (storageAccount != null)
            {
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(_imageContainers);
                var blob = container.GetBlockBlobReference(publicID.ToString());
                if (await blob.DeleteIfExistsAsync())
                    return true;

                return false;
            }

            return false;
        }


    }
}