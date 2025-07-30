using EP.Application.Common.DTOs.Auth;
using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Common.Interfaces.Services;
using MediatR;

namespace EP.Application.Commands.Auth
{
    public record LoginCommand: IRequest<LoginResponse>
    {
        public string EmailOrUsername { get; set; }
        public string Password { get; set; }
        public bool Remember { get; set; }
    }
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHashService _hashService;
        private readonly ITokenService _tokenService;
        public LoginCommandHandler(IUserRepository userRepository, IHashService hashService, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _hashService = hashService;
            _tokenService = tokenService;
        }
        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.EmailOrUsername) || string.IsNullOrEmpty(request.Password))
            {
                throw new ArgumentException("Vui lòng nhập đủ thông tin yêu cầu!");
            }
            var user = await _userRepository.GetUserByUsernameOrEmail(request.EmailOrUsername);
            if (user == null)
            {
                throw new ArgumentException("Thông tin đăng nhập không đúng!");
            }

            if (user.Status == false)
            {
                throw new ArgumentException("Tài khoản không khả dụng!");
            }

            if (!_hashService.Verify(user.Password, request.Password))
            {
                throw new ArgumentException("Thông tin đăng nhập không đúng!");
            }

            var accessToken = _tokenService.GenerateToken(user);
            var userResponse = await _userRepository.getAccountById(user.Id);

            return new LoginResponse
            {
                AccessToken = accessToken,
                User = userResponse
            };
        }
    }
}
