using HouseBrokerApplication.Domain.Aggregates.FileInfo;

namespace HouseBrokerApplication.Application.Abstractions.Services
{
    public interface IFileService
    {
        Task<Result<Domain.Aggregates.FileInfo.FileInfo>> UploadAsync(byte[] fileBytes, string fileName);
    }
}
