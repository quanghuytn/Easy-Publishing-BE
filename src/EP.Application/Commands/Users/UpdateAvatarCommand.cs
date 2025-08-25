using MediatR;

namespace EP.Application.Commands.Users
{
    public record UpdateAvatarCommand : IRequest<string>
    {
        public int UserId { get; set; }
        public Stream FileStream { get; set; } 
        public string FileName { get; set; }
    }
}
