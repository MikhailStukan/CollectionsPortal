using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace CollectionsPortal.CloudStorage
{
    public class GoogleCloudStorage : ICloudStorage
    {
        private readonly GoogleCredential googleCredential;
        private readonly StorageClient storageClient;
        private readonly string bucketName;

        public GoogleCloudStorage(IConfiguration configuration)
        {
            googleCredential = GoogleCredential.FromJson(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"));
            storageClient = StorageClient.Create(googleCredential);
            bucketName = configuration.GetValue<string>("BucketName");
        }

        public async Task<string> UploadFileAsync(IFormFile imageFile, string fileNameForStorage)
        {
            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                var dataObject = await storageClient.UploadObjectAsync(bucketName, fileNameForStorage, null, memoryStream);
                return dataObject.MediaLink;
            }
        }

        public async Task DeleteFileAsync(string fileNameForStorage)
        {
            await storageClient.DeleteObjectAsync(bucketName, fileNameForStorage);
        }
    }
}