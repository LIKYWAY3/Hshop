using ASPtestShop.Models.DTO.Upload;
using ASPtestShop.Services.Interfaces.Admin;

namespace ASPtestShop.Services.Implementations.Admin
{
    public class AdminUploadService : IAdminUploadService
    {
        private readonly IWebHostEnvironment _environment;

        public AdminUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<UploadImageResultDto> UploadProductImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new UploadImageResultDto
                {
                    Success = false,
                    Message = "Vui lòng chọn file ảnh"
                };
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return new UploadImageResultDto
                {
                    Success = false,
                    Message = "Chỉ cho phép upload file ảnh .jpg, .jpeg, .png, .webp"
                };
            }

            var maxFileSize = 5 * 1024 * 1024;

            if (file.Length > maxFileSize)
            {
                return new UploadImageResultDto
                {
                    Success = false,
                    Message = "File ảnh không được vượt quá 5MB"
                };
            }

            var webRootPath = _environment.WebRootPath;

            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            var uploadFolder = Path.Combine(webRootPath, "Uploads", "products");

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            var fileName = $"{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = $"/Uploads/products/{fileName}";

            return new UploadImageResultDto
            {
                Success = true,
                Message = "Upload ảnh thành công",
                ImageUrl = imageUrl
            };
        }
    }
}