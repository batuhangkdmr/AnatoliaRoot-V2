using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.IO;

namespace AnatoliaRoot_V2.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly string _cloudName;
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            _cloudName = configuration["Cloudinary:CloudName"];
            _apiKey = configuration["Cloudinary:ApiKey"];
            _apiSecret = configuration["Cloudinary:ApiSecret"];
            _cloudinary = new Cloudinary(new Account(_cloudName, _apiKey, _apiSecret));
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "products/anatoliaroot"
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl?.ToString();
        }
    }
} 