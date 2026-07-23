using Microsoft.AspNetCore.Http;

namespace InsuranceManagementSystem.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveClaimDocumentAsync(IFormFile file);
    }
}
