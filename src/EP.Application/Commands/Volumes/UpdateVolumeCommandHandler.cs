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
    public record UpdateVolumeCommand : IRequest<int>
    {
        public int VolumeId { get; set; }
        public string VolumeTitle { get; set; } = null!;
    }

    public class UpdateVolumeCommandValidator : AbstractValidator<UpdateVolumeCommand>
    {
        public UpdateVolumeCommandValidator()
        {
            RuleFor(command => command.VolumeId)
                .GreaterThan(0).WithMessage("VolumeId must be greater than 0.");

            RuleFor(command => command.VolumeTitle)
                .NotEmpty().WithMessage("VolumeTitle is required.")
                .MaximumLength(200).WithMessage("VolumeTitle must not exceed 200 characters.");
        }
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
            var volume = await _unitOfWork.VolumeRepository.GetByIdAsync(request.VolumeId);

            if (volume == null)
            {
                throw new KeyNotFoundException("Hệ thống xảy ra lỗi. Vui lòng thử lại sau!");
            }

            volume.VolumeTitle = request.VolumeTitle;
            await _unitOfWork.VolumeRepository.UpdateAsync(volume);

            return await _unitOfWork.CompleteAsync();
        }
    }
}
