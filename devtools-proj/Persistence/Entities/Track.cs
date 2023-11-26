using devtools_proj.Persistence.Entities.EntityInterfaces;
using MongoDB.Bson;

namespace devtools_proj.Persistence.Entities;

public class Track : IPiece, IHasId
{
    public Track(Artist author, List<Artist> artists, List<string> genres, int length, string name)
    {
        Author = author;
        Artists = artists;
        Genres = genres;
        Length = length;
        Name = name;
    }

    public ObjectId Id { get; set; }

    public Artist Author { get; set; }

    public List<Artist> Artists { get; set; }

    public List<string> Genres { get; set; }

    public int Length { get; set; }

    public string Name { get; set; }

    public override string ToString()
    {
        return $"{Author.Alias} - {Name}";
    }
}