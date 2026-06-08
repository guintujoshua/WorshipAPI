namespace Models.Songs;

public class Song
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public char Key { get; set; }
    public string Year { get; set; } = string.Empty;
    public List<string> LyricsWithChords { get; set; } = new();
    public bool Status { get; set; }
}
