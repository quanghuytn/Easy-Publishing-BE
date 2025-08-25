using EP.Application.Common.Interfaces;
using EP.Application.Common.Interfaces.Services;
using FluentValidation;
using MediatR;

namespace EP.Application.Commands.Auth
{
    public record ChangePasswordCommand : IRequest<int>
    {
        public int UserId { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(command => command.UserId)
                .GreaterThan(0).WithMessage("User không hợp lệ.");

            RuleFor(command => command.OldPassword)
                .NotEmpty().WithMessage("OldPassword is required.")
                .MinimumLength(6).WithMessage("OldPassword must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("OldPassword must not exceed 100 characters.");

            RuleFor(command => command.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters.");

            RuleFor(command => command.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm Password is required.")
                .Equal(command => command.Password).WithMessage("Passwords must match.");
        }
    }
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, int>
    {
        private readonly IHashService _hashService;
        private readonly IUnitOfWork _unitOfWork;
        public ChangePasswordCommandHandler(IUnitOfWork unitOfWork, IHashService hashService)
        {
            _unitOfWork = unitOfWork;
            _hashService = hashService;
        }
        public async Task<int> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);

            if (user == null) 
            {
                throw new Exception("Đổi mật khẩu thất bại. Xin vui lòng thử lại sau!");
            }

            if (!_hashService.Verify(user.Password, request.OldPassword))
            {
                throw new Exception("Mật khẩu không đúng");
            }

            _unitOfWork.UserRepository.ResetPassword(request.UserId, _hashService.Hash(request.Password));
            
            return await _unitOfWork.CompleteAsync();
        }
    }
}
