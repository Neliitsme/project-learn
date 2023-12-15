namespace devtools_proj.DTOs;

public class CreateTrackDto
{
    public CreateTrackDto(string author, List<string> artists, List<string> genres, int length, string name)
    {
        Author = author;
        Artists = artists;
        Genres = genres;
        Length = length;
        Name = name;
    }

    public string Author { get; set; }
    public List<string> Artists { get; set; }

    public List<string> Genres { get; set; }

    public int Length { get; set; }

    public string Name { get; set; }
}