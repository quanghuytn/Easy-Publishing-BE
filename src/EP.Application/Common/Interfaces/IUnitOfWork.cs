using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        IUserRepository User { get; }
        Task<int> CompleteAsync();
    }
}
