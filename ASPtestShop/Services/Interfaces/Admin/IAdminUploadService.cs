using ASPtestShop.Models.DTO.Upload;
using Microsoft.AspNetCore.Http;

namespace ASPtestShop.Services.Interfaces.Admin;

public interface IAdminUploadService
{
    Task<UploadImageResultDto> UploadProductImageAsync(IFormFile file);
}