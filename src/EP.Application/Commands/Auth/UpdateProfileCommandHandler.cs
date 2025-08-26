using EP.Application.Common.DTOs.User;
using EP.Application.Common.Interfaces;
using EP.Application.Common.Interfaces.Services.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Commands.Auth
{
    public record UpdateProfileCommand : IRequest<string>
    {
        public int UserId { get; set; }
        public string? UserFullname { get; set; }
        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? DescriptionMarkdown { get; set; }
        public string? DescriptionHTML { get; set; }
    }
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        public UpdateProfileCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }
        public async Task<string> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);

            if (user == null)
            {
                throw new InvalidOperationException("Lỗi hệ thống!");
            }
            try
            {
                user.UserFullname = request.UserFullname;
                user.Gender = request.Gender.ToLower().Equals("male");
                user.Phone = request.Phone;
                user.Dob = request.Dob;
                user.Address = request.Address;
                user.DescriptionMarkdown = request.DescriptionMarkdown;
                user.DescriptionHtml = request.DescriptionHTML;

                await _unitOfWork.UserRepository.UpdateAsync(user);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Cập nhật thất bại. Vui lòng thử lại sau!", ex);
            }
            

            UserDto userDTO = await _unitOfWork.UserRepository.GetUserByUsernameOrEmail(user.Email ?? user.Username);

            return _tokenService.GenerateToken(userDTO);
        }
    }
}
