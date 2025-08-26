using EP.Application.Common.Interfaces;
using EP.Application.Common.Interfaces.Services.Common;
using MediatR;

namespace EP.Application.Commands.Users
{
    public class UpdateAvatarCommandHandler(IFileStorageService fileStorageService, IUnitOfWork unitOfWork) : IRequestHandler<UpdateAvatarCommand, string>
    {
        private readonly IFileStorageService _fileStorageService = fileStorageService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<string> Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
        {
            var avatar = await _fileStorageService.UploadAvatarAsync(request.FileStream, request.FileName);
            
            if (string.IsNullOrEmpty(avatar))
            {
                throw new Exception("Failed to upload avatar.");
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            user.UserImage = avatar;
            await _unitOfWork.CompleteAsync();
            return avatar;
        }
    }
}
