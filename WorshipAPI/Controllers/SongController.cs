using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models.Songs;
using Services.Songs;

namespace WorshipAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SongController : ControllerBase
{
    private readonly ISongService _songService;

    public SongController(ISongService songService)
    {
        _songService = songService;
    }

    // GET: api/Song
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Song>>> GetAllSongs()
    {
        var songs = await _songService.GetAllSongsAsync();
        return Ok(songs);
    }

    // GET: api/Song/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Song>> GetSong(int id)
    {
        var song = await _songService.GetSongByIdAsync(id);

        if (song == null)
            return NotFound();

        return Ok(song);
    }

    // POST: api/Song
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Song>> CreateSong([FromBody] Song song)
    {
        var createdSong = await _songService.CreateSongAsync(song);
        return CreatedAtAction(nameof(GetSong), new { id = createdSong.Id }, createdSong);
    }

    // PUT: api/Song/5
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<Song>> UpdateSong(int id, [FromBody] Song song)
    {
        var updatedSong = await _songService.UpdateSongAsync(id, song);

        if (updatedSong == null)
            return NotFound();

        return Ok(updatedSong);
    }

    // DELETE: api/Song/5
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteSong(int id)
    {
        var result = await _songService.DeleteSongAsync(id);

        if (!result)
            return NotFound();

        return NoContent();
    }
}
