using EP.Application.Common;
using EP.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace EP.Application.Commands.Users
{
    public record SwitchUserStatusCommand(string Email) : IRequest<ApiResponse<string>>;
    public class SwitchUserStatusCommandValidator : AbstractValidator<SwitchUserStatusCommand>
    {
        public SwitchUserStatusCommandValidator()
        {
            RuleFor(command => command.Email)
                .NotNull().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }
    public class SwitchUserStatusCommandHandler : IRequestHandler<SwitchUserStatusCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public SwitchUserStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<string>> Handle(SwitchUserStatusCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.FindAsync(u => u.Email.Equals(request.Email));
            if (user == null) {
                return ApiResponse<string>.Failure("Tài khoản không tồn tại!");
            }
            string msg = "Kích hoạt tài khoản thành công!";

            if (user.Status == false || user.Status == null)
            {
                user.Status = true;
            }
            else
            {
                msg = "Khóa tài khoản thành công!";
                user.Status = false;
            }

            var affectedRows = await _unitOfWork.CompleteAsync();
            if (affectedRows > 0)
            {
                return ApiResponse<string>.Success(msg);
            }
            else
            {
                return ApiResponse<string>.Failure("Hệ thống xảy ra lỗi!. Vui lòng thử lại sau.");
            }
        }
    }
}
