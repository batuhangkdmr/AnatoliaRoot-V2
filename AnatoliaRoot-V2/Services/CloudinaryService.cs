using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.IO;
using System;
using System.Collections.Generic;

namespace AnatoliaRoot_V2.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private const long MaxFileSize = 5 * 1024 * 1024;
        private static readonly HashSet<string> AllowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };
        private static readonly HashSet<string> AllowedContentTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/webp"
        };
        private readonly string _cloudName;
        private readonly string _apiKey;
        private readonly string _apiSecret;

        public CloudinaryService(IConfiguration configuration)
        {
            _cloudName = configuration["Cloudinary:CloudName"];
            _apiKey = configuration["Cloudinary:ApiKey"];
            _apiSecret = configuration["Cloudinary:ApiSecret"];
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (string.IsNullOrWhiteSpace(_cloudName)
                || string.IsNullOrWhiteSpace(_apiKey)
                || string.IsNullOrWhiteSpace(_apiSecret))
            {
                throw new InvalidOperationException("Cloudinary yapılandırması eksik.");
            }

            if (file == null || file.Length == 0 || file.Length > MaxFileSize)
                return null;

            if (!AllowedExtensions.Contains(Path.GetExtension(file.FileName)) || !AllowedContentTypes.Contains(file.ContentType))
                return null;

            await using var stream = file.OpenReadStream();
            var header = new byte[12];
            var bytesRead = await stream.ReadAsync(header, 0, header.Length);
            stream.Position = 0;

            if (!HasValidImageSignature(header, bytesRead))
                return null;

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "products/anatoliaroot",
                UseFilename = false,
                UniqueFilename = true,
                Overwrite = false
            };
            var cloudinary = new Cloudinary(new Account(_cloudName, _apiKey, _apiSecret));
            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl?.ToString();
        }

        private static bool HasValidImageSignature(byte[] header, int length)
        {
            var isJpeg = length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;
            var isPng = length >= 8 && header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E
                && header[3] == 0x47 && header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A;
            var isWebP = length >= 12 && header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46
                && header[3] == 0x46 && header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50;

            return isJpeg || isPng || isWebP;
        }
    }
}
