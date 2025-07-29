using EP.Application.Common.Interfaces;
using EP.Application.Common.Interfaces.Services;
using MediatR;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EP.Application.Commands.Auth
{
    public record ResetPasswordCommand : IRequest<Unit>
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mailService;
        private readonly IHashService _hashService;
        public ResetPasswordCommandHandler(IUnitOfWork unitOfWork, IMailService mailService, IHashService hashService)
        {
            _unitOfWork = unitOfWork;
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _hashService = hashService;
        }
        public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameOrEmail(request.Email);

            if (user == null)
            {
                throw new ArgumentException("Email không tồn tại!");
            }

            if (!request.Password.Equals(request.ConfirmPassword))
            {
                throw new ArgumentException("Mật khẩu và xác nhận mật khẩu không khớp!");
            }

            try
            {
                _unitOfWork.UserRepository.ResetPassword(user.Id, _hashService.Hash(request.Password));
                _mailService.Send(request.Email,
                    "Easy Publishing: Đặt lại mật khẩu",
                    "<b>Xin chào " + user.Username + ",</b>" +
                    "<p>Mật khẩu của bạn đã được đặt lại thành công!</p> ");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Đặt lại mật khẩu thất bại. Vui lòng thử lại sau.", ex);
            }
            await _unitOfWork.CompleteAsync();

            return Unit.Value;
        }
    }
}
