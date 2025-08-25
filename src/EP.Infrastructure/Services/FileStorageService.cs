using EP.Application.Common.Interfaces.Services;

namespace EP.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        public async Task<string> UploadAvatarAsync(Stream fileStream, string fileName)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Assets/images/avatar");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var fileExtension = Path.GetExtension(fileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var uniqueFileName = fileNameWithoutExtension + DateTime.Now.ToString("yyyyMMddHHmmssffff") + fileExtension;
            var fullPath = Path.Combine(path, uniqueFileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream);
            }

            return uniqueFileName;
        }

        public async Task<string> UploadStoryImageAsync(Stream fileStream, string fileName, string previousFilename)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Assets/images/story");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // Xóa ảnh cũ nếu có
            if (!string.IsNullOrWhiteSpace(previousFilename))
            {
                string oldFilePath = Path.Combine(path, previousFilename);
                if (System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);
            }

            // Lưu ảnh mới
            string fileExtension = Path.GetExtension(fileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string uniqueFileName = $"{fileNameWithoutExtension}{DateTime.Now:yyyyMMddHHmmssffff}{fileExtension}";
            string fullPath = Path.Combine(path, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream);
            }

            return uniqueFileName;
        }
    }
}
