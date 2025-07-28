using EP.Application.Common.DTOs;
using EP.Application.Common.Interfaces.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EP.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        //private readonly IWebHostEnvironment _environment;

        //public FileStorageService(IWebHostEnvironment environment)
        //{
        //    _environment = environment;
        //}

        //public async Task<string> SaveAvatarAsync(Stream fileStream, string fileName)
        //{
        //    var uploadsPath = Path.Combine(_environment.WebRootPath, "Assets/images/avatar");
        //    if (!Directory.Exists(uploadsPath))
        //        Directory.CreateDirectory(uploadsPath);

        //    var fileExtension = Path.GetExtension(fileName);
        //    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        //    var uniqueFileName = $"{fileNameWithoutExtension}{DateTime.Now:yyyyMMddHHmmssffff}{fileExtension}";
        //    var filePath = Path.Combine(uploadsPath, uniqueFileName);

        //    await using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await fileStream.CopyToAsync(stream);
        //    }

        //    return uniqueFileName;
        //}
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
    }
}
