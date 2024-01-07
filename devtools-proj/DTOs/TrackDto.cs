using System.Diagnostics.CodeAnalysis;
using devtools_proj.Persistence.Entities;

namespace devtools_proj.DTOs;

public class TrackDto
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public TrackDto()
    {
    }

    public TrackDto(Track track)
    {
        Id = track.Id.ToString();
        Author = track.Author;
        Artists = track.Artists;
        Genres = track.Genres;
        Length = track.Length;
        Name = track.Name;
    }

    public string? Id { get; set; }

    // public Artist Author { get; set; }
    public string? Author { get; set; }

    // public List<Artist> Artists { get; set; }
    public List<string>? Artists { get; set; }

    public List<string>? Genres { get; set; }

    public int? Length { get; set; }

    public string? Name { get; set; }
}