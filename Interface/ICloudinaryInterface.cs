namespace E_Commerce.Interface
{
    public interface ICloudinaryInterface
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
