using EP.Application.Common;
using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using MediatR;

namespace EP.Application.Commands.Users
{
    public record AddNewUserCommand(User NewUser) : IRequest<User>;
    public class AddNewUserCommandHandler : IRequestHandler<AddNewUserCommand, User>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddNewUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User> Handle(AddNewUserCommand request, CancellationToken cancellationToken)
        {
            var user = request.NewUser;
            await _unitOfWork.UserRepository.AddAsync(user);
            var affectedRows = await _unitOfWork.CompleteAsync();
            if (affectedRows > 0)
            {
                return user;
            }
            else
            {
                return null;
            }
        }
    }
}
