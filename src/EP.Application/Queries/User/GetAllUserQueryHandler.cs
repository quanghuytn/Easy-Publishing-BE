using EP.Application.Common;
using EP.Application.Common.DTOs.User;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.User
{
    public record GetAllUserQuery : IRequest<ApiResponse<List<UserDto2>>>;
    public class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, ApiResponse<List<UserDto2>>>
    {
        private readonly IUserRepository _userRepository;
        public GetAllUserQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<ApiResponse<List<UserDto2>>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            var userList = await _userRepository.GetAllUsers();

            return ApiResponse<List<UserDto2>>.Success("Sucess", userList);
        }
    }
}
