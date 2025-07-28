using EP.Application.Common.Interfaces;
using EP.Infrastructure.Data;
using EP.Infrastructure.Repositories;

namespace EP.Infrastructure.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Context _context;
        public ICategoryRepository Category { get; private set; }
        public IUserRepository User { get; private set; }

        public UnitOfWork(Context context)
        {
            _context = context;
            Category = new CategoryRepository(context);
            User = new UserRepository(context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
