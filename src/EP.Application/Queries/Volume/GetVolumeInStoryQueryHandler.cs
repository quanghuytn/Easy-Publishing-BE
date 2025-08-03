using EP.Application.Common;
using EP.Application.Common.DTOs.Volume;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;
using System.Collections.Generic;

namespace EP.Application.Queries.Volume
{
    public record GetVolumeInStoryQuery : IRequest<ApiResponse<IEnumerable<VolumeDto>>>
    {
        public int StoryId { get; set; }
    }
    public class GetVolumeInStoryQueryHandler : IRequestHandler<GetVolumeInStoryQuery, ApiResponse<IEnumerable<VolumeDto>>>
    {
        private readonly IVolumeRepository _volumeRepository;
        public GetVolumeInStoryQueryHandler(IVolumeRepository volumeRepository)
        {
            _volumeRepository = volumeRepository ?? throw new ArgumentNullException(nameof(volumeRepository));
        }
        public async Task<ApiResponse<IEnumerable<VolumeDto>>> Handle(GetVolumeInStoryQuery request, CancellationToken cancellationToken)
        {
            var volumes = await _volumeRepository.SelectWithConditionAsync(v => v.StoryId == request.StoryId, 
                                                                    v => new VolumeDto
                                                                    {
                                                                        VolumeId = v.VolumeId,
                                                                        VolumeNumber = v.VolumeNumber,
                                                                        VolumeTitle = v.VolumeTitle
                                                                    });
            return ApiResponse<IEnumerable<VolumeDto>>.Success("Danh sách tập", volumes);
        }
    }
}
