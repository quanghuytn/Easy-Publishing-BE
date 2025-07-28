using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Commands.User
{
    public record UpdateAvatarCommand : IRequest<string>
    {
        public int UserId { get; set; }
        public Stream FileStream { get; set; } 
        public string FileName { get; set; }
    }
}
