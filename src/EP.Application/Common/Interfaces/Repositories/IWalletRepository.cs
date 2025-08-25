using EP.Application.Common.DTOs.Wallet;
using EP.Domain.Models;

namespace EP.Application.Common.Interfaces.Repositories
{
    public interface IWalletRepository : IRepository<Wallet>
    {
        Task<UserWalletDto?> GetUserWallet(int userId);
    }
}
