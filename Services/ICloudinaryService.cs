using Microsoft.AspNetCore.Http;

namespace InsuranceManagementSystem.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);

        Task DeleteImageAsync(string imageUrl);
    }
}