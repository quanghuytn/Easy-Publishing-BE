using EP.Application.Common;
using EP.Application.Common.DTOs.Volume;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Volume
{
    public record GetVolumesQuery(int storyId) : IRequest<ApiResponse<IEnumerable<VolumeChapterDto>>>;
    public class GetVolumesQueryHandler : IRequestHandler<GetVolumesQuery, ApiResponse<IEnumerable<VolumeChapterDto>>>
    {
        private readonly IVolumeRepository _volumeRepository;
        public GetVolumesQueryHandler(IVolumeRepository volumeRepository)
        {
            _volumeRepository = volumeRepository ?? throw new ArgumentNullException(nameof(volumeRepository));
        }
        public async Task<ApiResponse<IEnumerable<VolumeChapterDto>>> Handle(GetVolumesQuery request, CancellationToken cancellationToken)
        {
            var volumes = await _volumeRepository.GetVolumes(request.storyId);

            return ApiResponse<IEnumerable<VolumeChapterDto>>.Success("Danh sách tập cụ thể", volumes);
        }
    }
}
