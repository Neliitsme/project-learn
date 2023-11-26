using devtools_proj.Persistence.Entities.EntityInterfaces;
using MongoDB.Bson;

namespace devtools_proj.Persistence.Entities;

/// <summary>
///     Real human bean and a real hero
/// </summary>
public class Artist : IHasId
{
    public Artist(string alias, string firstName, string lastName
    )
    {
        Alias = alias;
        FirstName = firstName;
        LastName = lastName;
    }

    public string Alias { get; set; }

    /// <summary>
    ///     First name of the bean.
    /// </summary>
    private string FirstName { get; }

    /// <summary>
    ///     Last name of the bean.
    /// </summary>
    private string LastName { get; }

    /// <summary>
    ///     Full name including the alias. For search optimization
    /// </summary>
    public string FullName => $"{FirstName} {LastName} {Alias}";

    public List<IPiece> ReleasedTracks { get; set; } = new();

    public List<string> TrackGenres => ReleasedTracks.SelectMany(rt => rt.Genres).Distinct().ToList();

    public ObjectId Id { get; set; }

    public override string ToString()
    {
        return Alias;
    }
}