using Dapper;
using Models.Songs;
using Providers.Database;
using System.Data;
using System.Text.Json;

namespace Providers.Songs;

public class SongProvider : ISongProvider
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SongProvider(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Song>> GetAllSongsAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var songs = await connection.QueryAsync<SongDto>(
            "sp_GetAllSongs",
            commandType: CommandType.StoredProcedure);

        return songs.Select(MapToSong);
    }

    public async Task<Song?> GetSongByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var songDto = await connection.QuerySingleOrDefaultAsync<SongDto>(
            "sp_GetSongById",
            new { p_Id = id },
            commandType: CommandType.StoredProcedure);

        return songDto != null ? MapToSong(songDto) : null;
    }

    public async Task<Song> CreateSongAsync(Song song)
    {
        using var connection = _connectionFactory.CreateConnection();
        var lyricsJson = JsonSerializer.Serialize(song.LyricsWithChords);

        var result = await connection.QuerySingleAsync<int>(
            "sp_CreateSong",
            new
            {
                p_Title = song.Title,
                p_Artist = song.Artist,
                p_Key = song.Key.ToString(),
                p_Year = song.Year,
                p_LyricsWithChords = lyricsJson,
                p_Status = song.Status
            },
            commandType: CommandType.StoredProcedure);

        song.Id = result;
        return song;
    }

    public async Task<Song?> UpdateSongAsync(int id, Song song)
    {
        using var connection = _connectionFactory.CreateConnection();
        var lyricsJson = JsonSerializer.Serialize(song.LyricsWithChords);

        var result = await connection.QuerySingleOrDefaultAsync<int>(
            "sp_UpdateSong",
            new
            {
                p_Id = id,
                p_Title = song.Title,
                p_Artist = song.Artist,
                p_Key = song.Key.ToString(),
                p_Year = song.Year,
                p_LyricsWithChords = lyricsJson
            },
            commandType: CommandType.StoredProcedure);

        if (result == 0)
            return null;

        song.Id = id;
        return song;
    }

    public async Task<bool> DeleteSongAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<int>(
            "sp_DeleteSong",
            new { p_Id = id },
            commandType: CommandType.StoredProcedure);

        return result > 0;
    }

    // Helper method to map DTO to Song entity
    private static Song MapToSong(SongDto dto)
    {
        var lyrics = string.IsNullOrEmpty(dto.LyricsWithChords)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(dto.LyricsWithChords) ?? new List<string>();

        return new Song
        {
            Id = dto.Id,
            Title = dto.Title,
            Artist = dto.Artist,
            Key = dto.Key,
            Year = dto.Year,
            LyricsWithChords = lyrics,
            Status = dto.Status
        };
    }

    // DTO for Dapper mapping
    private class SongDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public char Key { get; set; }
        public string Year { get; set; } = string.Empty;
        public string LyricsWithChords { get; set; } = string.Empty; // JSON string from database
        public bool Status { get; set; }
    }
}
