namespace E_Commerce.Interface
{
    public interface CloudinaryInterface
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
