using EP.Application.Common;
using EP.Application.Common.DTOs.Volume;
using EP.Application.Common.Interfaces.Repositories;
using EP.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Queries.Reviews
{
    public record GetVolumeReviewQuery : IRequest<ApiResponse<IEnumerable<VolumeReviewDto>>>
    {
        public int StoryId { get; set; }
        public int UserId { get; set; }
    }
    public class GetVolumeReviewQueryHandler : IRequestHandler<GetVolumeReviewQuery, ApiResponse<IEnumerable<VolumeReviewDto>>>
    {
        private readonly IVolumeRepository _volumeRepository;
        public GetVolumeReviewQueryHandler(IVolumeRepository volumeRepository)
        {
            _volumeRepository = volumeRepository ?? throw new ArgumentNullException(nameof(volumeRepository));
        }
        public async Task<ApiResponse<IEnumerable<VolumeReviewDto>>> Handle(GetVolumeReviewQuery request, CancellationToken cancellationToken)
        {
            var volumeReviews = await _volumeRepository.GetVolumeReview(request.StoryId, request.UserId);
            if (!volumeReviews.Any())
            {
                return ApiResponse<IEnumerable<VolumeReviewDto>>.Failure("Truyện không có tập nào cần review.");
            }

            return ApiResponse<IEnumerable<VolumeReviewDto>>.Success("Danh sách các tập của truyện", volumeReviews);
        }
    }
}
