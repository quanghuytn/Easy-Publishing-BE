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
    public record AddVolumeCommand : IRequest<int>
    {
        public int StoryId { get; init; }
        public string VolumeTitle { get; init; } = null!;
    }
    public class AddVolumeCommandHandler : IRequestHandler<AddVolumeCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddVolumeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<int> Handle(AddVolumeCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.VolumeTitle))
            {
                throw new ArgumentException("Volume title cannot be null or empty.", nameof(request.VolumeTitle));
            }

            var latestVolumeNumber = await _unitOfWork.VolumeRepository.GetLatestVolumeNumber(request.StoryId);

            if (latestVolumeNumber >= 2)
            {
                if (!await _unitOfWork.VolumeRepository.HasValidPreviousVolumeAsync(request.StoryId, latestVolumeNumber))
                {
                    throw new InvalidOperationException("The latest volume must have at least two chapters before adding a new volume.");
                }
            }

            Volume volume = new Volume()
            {
                StoryId = request.StoryId,
                VolumeTitle = request.VolumeTitle,
                VolumeNumber = latestVolumeNumber + 1,
                CreateTime = DateTime.Now
            };

            await _unitOfWork.VolumeRepository.AddAsync(volume);

            return await _unitOfWork.CompleteAsync();
        }
    }
}
