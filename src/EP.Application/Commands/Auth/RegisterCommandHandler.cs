using EP.Application.Common.Interfaces;
using EP.Application.Common.Interfaces.Services;
using FluentValidation;
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

    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(command => command.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(command => command.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(50).WithMessage("Username must not exceed 50 characters.");

            RuleFor(command => command.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters.");

            RuleFor(command => command.ConfirmPassword)
                .NotEmpty().WithMessage("ConfirmPassword is required.")
                .Equal(command => command.Password).WithMessage("Passwords must match.");
        }
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
