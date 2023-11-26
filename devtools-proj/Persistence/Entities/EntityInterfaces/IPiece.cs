namespace devtools_proj.Persistence.Entities.EntityInterfaces;

public interface IPiece : INamed
{
    /// <summary>
    ///     The one who released the track
    /// </summary>
    public Artist Author { get; set; }

    /// <summary>
    ///     A list of guest-artists, co-authors, etc. Includes author.
    /// </summary>
    public List<Artist> Artists { get; set; }

    /// <summary>
    ///     Self-explanatory, genres of the track
    /// </summary>
    public List<string> Genres { get; set; }

    /// <summary>
    ///     Self-explanatory, the length of the track. In seconds
    /// </summary>
    public int Length { get; set; }
}