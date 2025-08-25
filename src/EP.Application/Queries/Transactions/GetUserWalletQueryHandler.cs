using EP.Application.Common;
using EP.Application.Common.DTOs.Wallet;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Transactions
{
    public record GetUserWalletQuery(int UserId) : IRequest<ApiResponse<UserWalletDto>>;
    public class GetUserWalletQueryHandler : IRequestHandler<GetUserWalletQuery, ApiResponse<UserWalletDto>>
    {
        private readonly IWalletRepository _walletRepository;
        public GetUserWalletQueryHandler(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public async Task<ApiResponse<UserWalletDto>> Handle(GetUserWalletQuery request, CancellationToken cancellationToken)
        {
            var userWallet = await _walletRepository.GetUserWallet(request.UserId);
            if (userWallet == null) {
                throw new Exception("Ví không tồn tại!");
            }

            return ApiResponse<UserWalletDto>.Success("Ví người dùng", userWallet);
        }
    }
}
