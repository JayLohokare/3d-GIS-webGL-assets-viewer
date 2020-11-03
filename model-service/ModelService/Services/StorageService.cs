using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;

namespace ModelService.Services
{
    public class StorageService : IStorageSevice
    {
        private BlobServiceClient _blobServiceClient;
        private BlobContainerClient _containerClient;
        public StorageService(string connectionString, string containerName)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        }

        public byte[] GetBlobFile(string filePath)
        {
            var blobClient = _containerClient.GetBlobClient(filePath);

            // Download the blob's contents and save it to a file
            BlobDownloadInfo download = blobClient.Download();
            using (MemoryStream downloadMemoryStream = new MemoryStream())
            {
                download.Content.CopyTo(downloadMemoryStream);
                byte[] result = DownloadBlobAsByeArray(downloadMemoryStream);
                return result;
            }

        }

        public byte[] DownloadBlobAsByeArray(MemoryStream inputStream)
        {
            byte[] buffer = new byte[16 * 1024];

            inputStream.Position = 0; // Add this line to set the input stream position to 0

            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
