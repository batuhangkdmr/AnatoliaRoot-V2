using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AnatoliaRoot_V2.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
} 