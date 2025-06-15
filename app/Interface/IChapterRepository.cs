using app.DTOs.Chapter;
using app.Models;

namespace app.Interface
{
    public interface IChapterRepository
    {
        Task<List<MinimalChapterDto>> GetStoryChapters(int storyId);
        Task<bool> AddVolume(AddVolumeDto newVolume);
        Task<bool> UpdateVolume(VolumeDto volume);
        Task<List<VolumeDto>> GetVolumesByStory(int storyId);
        Task<List<VolumeChapterDto>> GetVolumes(int storyId);
        Task<bool> UpdateChapter(UpdateChapterDto chapter);
        Task<bool> DeleteChapter(int chapterId);
        Task AddChapter(AddChapterDto chapter);
        Task<ChapterDto?> GetChapterInfor(int chapterId);
        Task<ChapterContentDto?> GetChapterContent(int userId, long chapterNumber, int storyId);
        Task<bool> CheckReadPermission(int userId, int storyId, int chapterId);
        Task<Chapter?> GetChapter(int chapterId);
    }
}
