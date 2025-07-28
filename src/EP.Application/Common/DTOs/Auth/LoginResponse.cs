using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Auth
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public AccountDto? User { get; set; }
    }
}
