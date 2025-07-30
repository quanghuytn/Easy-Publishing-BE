using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Commands.Volumes
{
    public record UpdateVolumeCommand : IRequest<int>
    {
        public int VolumeId { get; set; }
        public string VolumeTitle { get; set; } = null!;
        public int VolumeNumber { get; set; }
    }
    public class UpdateVolumeCommandHandler : IRequestHandler<UpdateVolumeCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateVolumeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<int> Handle(UpdateVolumeCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.VolumeTitle))
            {
                throw new ArgumentException("Volume title cannot be null or empty.", nameof(request.VolumeTitle));
            }

            var volume = await _unitOfWork.VolumeRepository.GetByIdAsync(request.VolumeId);
            if (volume == null)
            {
                throw new KeyNotFoundException("Hệ thống xảy ra lỗi. Vui lòng thử lại sau!");
            }

            volume.VolumeTitle = request.VolumeTitle;
            volume.VolumeNumber = request.VolumeNumber;
            await _unitOfWork.VolumeRepository.UpdateAsync(volume);

            return await _unitOfWork.CompleteAsync();
        }
    }
}
