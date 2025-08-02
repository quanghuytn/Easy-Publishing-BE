using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using FluentValidation;
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

    public class AddVolumeCommandValidator : AbstractValidator<AddVolumeCommand>
    {
        public AddVolumeCommandValidator()
        {
            RuleFor(command => command.StoryId)
                .GreaterThan(0).WithMessage("StoryId không hợp lệ.");

            RuleFor(command => command.VolumeTitle)
                .NotEmpty().WithMessage("Volume Title is required.")
                .MaximumLength(200).WithMessage("Tiêu đề tập không được vượt quá 200 ký tự.");
        }
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
