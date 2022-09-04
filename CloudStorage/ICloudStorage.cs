namespace CollectionsPortal.CloudStorage
{
    public interface ICloudStorage
    {

        Task<string> UploadFileAsync(IFormFile imageFile, string fileNameForStorage);
        Task DeleteFileAsync(string fileNameForStorage);

    }
}
