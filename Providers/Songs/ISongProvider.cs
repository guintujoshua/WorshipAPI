using Models.Songs;

namespace Providers.Songs;

public interface ISongProvider
{
    Task<IEnumerable<Song>> GetAllSongsAsync();
    Task<Song?> GetSongByIdAsync(int id);
    Task<Song> CreateSongAsync(Song song);
    Task<Song?> UpdateSongAsync(int id, Song song);
    Task<bool> DeleteSongAsync(int id);
}
