using EP.Application.Common;
using EP.Application.Common.DTOs.Auth;
using EP.Application.Common.DTOs.Category;
using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Queries.Category;
using MediatR;

namespace EP.Application.Queries.User
{
    public record GetAccountQuery : IRequest<ApiResponse<AccountDto>>
    {
        public int UserId { get; set; }
    }
    public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, ApiResponse<AccountDto>>
    {
        private readonly IUserRepository _userRepository;
        public GetAccountQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<AccountDto>> Handle(GetAccountQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.getAccountById(request.UserId);
            if (user == null)
            {
                throw new ArgumentException("Tài khoản không tồn tại.");
            }
            return ApiResponse<AccountDto>.Success("Thông tin tài khoản", user);
        }
    }
}
