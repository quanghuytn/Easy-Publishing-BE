using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Common.Interfaces.Services.Common;
using MediatR;

namespace EP.Application.Queries.Auth
{
    public record SendMailConfirmQuery : IRequest<Unit>
    {
        public string? Email { get; set; }
    }
    public class SendMailConfirmQueryHandler : IRequestHandler<SendMailConfirmQuery, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMailService _mailService;
        private readonly ITokenService _tokenService;
        public SendMailConfirmQueryHandler(IUserRepository userRepository, IMailService mailService, ITokenService tokenService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _tokenService = tokenService;
        }
        public async Task<Unit> Handle(SendMailConfirmQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByUsernameOrEmail(request.Email);
            if (user == null)
            {
                throw new ArgumentException("Email chưa được đăng ký!");
            }
            try
            {
                string token = _tokenService.CreateForgotPasswordToken(request.Email);
                _mailService.Send(request.Email,
                        "Easy Publishing: Đặt lại mật khẩu",
                        "<b>Xin chào " + user.Username + ",</b>" +
                        "<p>Chúng tôi đã nhận được một yêu cầu đặt lại mật khẩu! </p> " +
                        "<p>Vui lòng bỏ qua mail này nếu bạn không phải người thực hiện.</p> " +
                        "<p>Nếu bạn là người thực hiện yêu cầu, vui lòng click vào đường dẫn dưới đây để đặt lại mật khẩu:</p> " +
                        "<a href =\"https://easy-publishing.vercel.app/auth/reset-password?token=" + token + "\">Đặt lại mật khẩu</a>");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Gửi email xác nhận thất bại. Vui lòng thử lại sau.", ex);
            }

            return Unit.Value;
        }
    }
}
