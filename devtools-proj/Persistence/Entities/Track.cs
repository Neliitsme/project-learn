using devtools_proj.DTOs;
using devtools_proj.Persistence.Entities.EntityInterfaces;
using MongoDB.Bson;

namespace devtools_proj.Persistence.Entities;

public class Track : IPiece, IHasId
{
    public Track(string author, List<string> artists, List<string> genres, int length, string name)
    {
        Author = author;
        Artists = artists;
        Genres = genres;
        Length = length;
        Name = name;
    }

    public Track(CreateTrackDto createTrackDto)
    {
        Author = createTrackDto.Author;
        Artists = createTrackDto.Artists;
        Genres = createTrackDto.Genres;
        Length = createTrackDto.Length;
        Name = createTrackDto.Name;
    }

    public ObjectId Id { get; set; }

    public string Author { get; set; }

    public List<string> Artists { get; set; }

    public List<string> Genres { get; set; }

    public int Length { get; set; }

    public string Name { get; set; }

    public override string ToString()
    {
        return $"{Author} - {Name}";
    }
}