using EP.Application.Common.DTOs.Volume;
using EP.Domain.Models;

namespace EP.Application.Common.Interfaces.Repositories
{
    public interface IVolumeRepository : IRepository<Volume>
    {
        Task<int> GetLatestVolumeNumber(int storyId);
        Task<bool> HasValidPreviousVolumeAsync(int storyId, int previousVolumeNumber);
        Task<IEnumerable<VolumeChapterDto>> GetVolumes(int storyId);
        Task<IEnumerable<VolumeReviewDto>> GetVolumeReview(int storyId, int userId);
    }
}
