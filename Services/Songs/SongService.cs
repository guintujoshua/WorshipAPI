using Models.Songs;
using Providers.Songs;

namespace Services.Songs;

public class SongService : ISongService
{
    private readonly ISongProvider _songProvider;

    public SongService(ISongProvider songProvider)
    {
        _songProvider = songProvider;
    }

    public async Task<IEnumerable<Song>> GetAllSongsAsync()
    {
        return await _songProvider.GetAllSongsAsync();
    }

    public async Task<Song?> GetSongByIdAsync(int id)
    {
        return await _songProvider.GetSongByIdAsync(id);
    }

    public async Task<Song> CreateSongAsync(Song song)
    {
        return await _songProvider.CreateSongAsync(song);
    }

    public async Task<Song?> UpdateSongAsync(int id, Song song)
    {
        return await _songProvider.UpdateSongAsync(id, song);
    }

    public async Task<bool> DeleteSongAsync(int id)
    {
        return await _songProvider.DeleteSongAsync(id);
    }
}
