using devtools_proj.DTOs;
using devtools_proj.Persistence.Entities;

namespace devtools_proj.Services;

public interface ISearchService
{
    public Task<IEnumerable<Track>> GetTracks();

    public Track CreateTrack(TrackDto trackDto);

    public Task<IEnumerable<Artist>> GetArtists();

    public Track CreateArtist(ArtistDto artistDto);
}