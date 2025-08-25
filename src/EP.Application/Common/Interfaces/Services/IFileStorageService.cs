using EP.Application.Common.DTOs;

namespace EP.Application.Common.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<string> UploadAvatarAsync(Stream fileStream, string fileName);
        Task<string> UploadStoryImageAsync(Stream fileStream, string fileName, string previousFilename);
    }
}
