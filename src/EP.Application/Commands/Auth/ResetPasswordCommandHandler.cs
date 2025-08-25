using EP.Application.Common.Interfaces;
using EP.Application.Common.Interfaces.Services;
using FluentValidation;
using MediatR;
using System.Security.Claims;

namespace EP.Application.Commands.Auth
{
    public record ResetPasswordCommand : IRequest<(int result, string email)>
    {
        public string Token { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(command => command.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters.");

            RuleFor(command => command.ConfirmPassword)
                .NotEmpty().WithMessage("Confir mPassword is required.")
                .Equal(command => command.Password).WithMessage("Passwords must match.");
        }
    }
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, (int result, string email)>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mailService;
        private readonly IHashService _hashService;
        private readonly ITokenService _tokenService;
        public ResetPasswordCommandHandler(IUnitOfWork unitOfWork, IMailService mailService, IHashService hashService, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _hashService = hashService;
            _tokenService = tokenService;
        }
        public async Task<(int result, string email)> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            string email;
            try
            {
                var principal = _tokenService.DecodeToken(request.Token);
                email = principal.FindFirst(ClaimTypes.Email)?.Value;
            }
            catch (Exception ex)
            {
                throw new Exception("Cập nhật password thất bại. Vui lòng thử lại sau!", ex);
            }


            var user = await _unitOfWork.UserRepository.GetUserByUsernameOrEmail(email);

            if (user == null)
            {
                throw new ArgumentException("Email không tồn tại!");
            }

            try
            {
                _unitOfWork.UserRepository.ResetPassword(user.Id, _hashService.Hash(request.Password));
                _mailService.Send(email,
                    "Easy Publishing: Đặt lại mật khẩu",
                    "<b>Xin chào " + user.Username + ",</b>" +
                    "<p>Mật khẩu của bạn đã được đặt lại thành công!</p> ");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Đặt lại mật khẩu thất bại. Vui lòng thử lại sau.", ex);
            }

            var result = await _unitOfWork.CompleteAsync();
            return (result, email);
        }
    }
}
