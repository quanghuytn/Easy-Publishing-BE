using EP.Application.Common;
using EP.Application.Common.DTOs.Author;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Queries.Author
{
    public record GetAuthorByIdQuery(int AuthorId) : IRequest<ApiResponse<AuthorDto>>;
    public class GetAuthorByIdQueryHandler : IRequestHandler<GetAuthorByIdQuery, ApiResponse<AuthorDto>>
    {
        private readonly IAuthorRepository _authorRepository;
        public GetAuthorByIdQueryHandler(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }
        public async Task<ApiResponse<AuthorDto>> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
        {
            var author = await  _authorRepository.GetAuthorById(request.AuthorId);
            if(author == null)
            {
                return ApiResponse<AuthorDto>.Failure("Tác giả không tồn tại");
            }

            return ApiResponse<AuthorDto>.Success("Thông tin tác giả", author);
        }
    }
}
