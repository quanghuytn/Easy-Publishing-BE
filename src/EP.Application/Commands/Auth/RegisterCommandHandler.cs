using EP.Application.Common.Interfaces;
using EP.Application.Common.Interfaces.Services;
using MediatR;

namespace EP.Application.Commands.Auth
{
    public record RegisterCommand : IRequest<int>
    {
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHashService _hashService;
        public RegisterCommandHandler(IUnitOfWork unitOfWork, IHashService hashService)
        {
            _unitOfWork = unitOfWork;
            _hashService = hashService ?? throw new ArgumentNullException(nameof(hashService));
        }
        public async Task<int> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Username))
            {
                throw new ArgumentException("Vui lòng nhập đủ thông tin yêu cầu!");
            }

            var user = await _unitOfWork.UserRepository.GetUserByUsernameOrEmail(request.Email);
            if (user != null)
            {
                throw new ArgumentException("Email đã được đăng ký bởi tài khoản khác!");
            }

            user = await _unitOfWork.UserRepository.GetUserByUsernameOrEmail(request.Username);
            if (user != null)
            {
                throw new ArgumentException("Username đã được đăng ký bởi tài khoản khác!");
            }

            if (!request.Password.Equals(request.ConfirmPassword))
            {
                throw new ArgumentException("Xác nhận mật khẩu không khớp với mật khẩu đã nhập!");
            }

            string passwordHash = _hashService.Hash(request.Password);
            try
            {
                await _unitOfWork.UserRepository.AddAsync(new Domain.Models.User
                {
                    Email = request.Email,
                    Password = passwordHash,
                    Username = request.Username,
                    Gender = true
                });
            }
            catch (Exception)
            {
                throw new ApplicationException("Đăng ký tài khoản thất bại. Vui lòng thử lại sau.");
            }

            return await _unitOfWork.CompleteAsync();
        }
    }
}
