using EP.Application.Common;
using EP.Application.Common.Interfaces;
using EP.Application.Common.Interfaces.Services.Common;
using MediatR;

namespace EP.Application.Commands.Stories
{
    public record UploadStoryImageCommand : IRequest<ApiResponse<string>>
    {
        public int? StoryId { get; set; }
        public int? UserId { get; set; }
        public Stream FileStream { get; set; }
        public string FileName { get; set; }
        public string? PreviousFilename { get; set; }
    }
    public class UploadStoryImageCommandHandler : IRequestHandler<UploadStoryImageCommand, ApiResponse<string>>
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;
        public UploadStoryImageCommandHandler(IFileStorageService fileStorageService, IUnitOfWork unitOfWork)
        {
            _fileStorageService = fileStorageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<string>> Handle(UploadStoryImageCommand request, CancellationToken cancellationToken)
        {
            var storyImage = await _fileStorageService.UploadStoryImageAsync(request.FileStream, request.FileName, request.PreviousFilename);

            if(request.StoryId != 0 && request.UserId != 0)
            {
                var story = await _unitOfWork.StoryRepository.FindAsync(s => s.StoryId == request.StoryId && s.AuthorId == request.UserId);
                if(story == null)
                {
                    return ApiResponse<string>.Failure("Cập nhật ảnh thất bại. Vui lòng thử lại sau!");
                }

                story.StoryImage = storyImage;
                await _unitOfWork.StoryRepository.UpdateAsync(story);
                var affectedRows = await _unitOfWork.CompleteAsync();

                if (affectedRows > 0)
                {
                    return ApiResponse<string>.Success("Upload ảnh thành công", storyImage);
                }
                else
                {
                    return ApiResponse<string>.Failure("Upload ảnh thất bại.");
                }
            }

            return ApiResponse<string>.Success("Upload ảnh thành công", storyImage);
        }
    }
}
