using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using E_Commerce.Interface;

namespace E_Commerce.Service
{
    public class CloudnaryService : ICloudinaryInterface
    {
        private readonly Cloudinary _cloudinary;

        public CloudnaryService(IConfiguration configuration)
        {
            var cloudinaryurl = configuration["Cloudinary:CLOUDINARY_URL"];

            if (!string.IsNullOrEmpty(cloudinaryurl))
            {
                // Use Cloudinary constructor with URL directly (no Account needed)
                _cloudinary = new Cloudinary(cloudinaryurl);
            }
            else
            {
                throw new Exception("Cloudinary configuration is missing.");
            }
        }
        

        public async Task<string> UploadImageAsync(IFormFile file)
        {

            try
            {
            if(file == null)
                {
                    throw new InvalidOperationException("File is null or empty");

                }
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.Name, stream),
                    UseFilename = true,
                    UniqueFilename = false,
                    Overwrite = true,
                    Folder = "Shopping Cart"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult == null || string.IsNullOrEmpty(uploadResult.SecureUrl?.ToString()))
                {
                    throw new InvalidOperationException("Cloudinary upload failed: No SecureUrl returned.");
                }


                return uploadResult.SecureUrl.ToString();
            }
              
            catch (Exception ex)
            {
                {

                    throw new InvalidOperationException("Upload failed", ex);
                }
              

            }
        }
    }
}
