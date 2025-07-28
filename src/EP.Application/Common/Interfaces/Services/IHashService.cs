using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.Interfaces.Services
{
    public interface IHashService
    {
        string Hash(string password);
        bool Verify(string passwordHash, string passwordInput);
    }
}
