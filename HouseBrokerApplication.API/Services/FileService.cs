using HouseBrokerApplication.API.Configurations.ConfigModels;
using HouseBrokerApplication.Application.Abstractions.Services;
using HouseBrokerApplication.Domain.Base;
using Microsoft.Extensions.Options;

namespace HouseBrokerApplication.API.Services
{
    public class FileService : IFileService
    {
        private readonly string _uploadDirectory;
        private readonly string _webRootPath;
        private readonly IRepository<Domain.Aggregates.FileInfo.FileInfo> _fileRepository;
        public FileService(
            IWebHostEnvironment webHostEnvironment,
            IOptions<FileUploadConfig> config,
            IRepository<Domain.Aggregates.FileInfo.FileInfo> fileRepository)
        {
            _webRootPath = webHostEnvironment.WebRootPath ??
                           Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot");

            _uploadDirectory = Path.Combine(_webRootPath, config.Value.UploadPath);
            _fileRepository = fileRepository;
        }

        public async Task<Result<Domain.Aggregates.FileInfo.FileInfo>> UploadAsync(byte[] fileBytes, string fileName)
        {
            if (!Directory.Exists(_uploadDirectory))
            {
                Directory.CreateDirectory(_uploadDirectory);
            }

            string originalFileName = Path.GetFileName(fileName);
            string storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            string filePath = Path.Combine(_uploadDirectory, storedFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await stream.WriteAsync(fileBytes);
            }
            string publicUrl = $"/{Path.GetFileName(_uploadDirectory)}/{storedFileName}";
            var fileInfo = new Domain.Aggregates.FileInfo.FileInfo(originalFileName, storedFileName, publicUrl);
            _fileRepository.Add(fileInfo);

            var isSuccess = await _fileRepository.UnitOfWork.SaveChangesAsync();
            if (!isSuccess) return Result<Domain.Aggregates.FileInfo.FileInfo>.Failure("Failed to upload the file");
            return Result<Domain.Aggregates.FileInfo.FileInfo>.Success("Successfull", fileInfo);
        }
    }
}
