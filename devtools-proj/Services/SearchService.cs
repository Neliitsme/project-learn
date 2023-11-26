using devtools_proj.DTOs;
using devtools_proj.Persistence;
using devtools_proj.Persistence.Entities;
using MongoDB.Driver;

namespace devtools_proj.Services;

public class SearchService : ISearchService
{
    private readonly IDbContext _db;

    public SearchService(IDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Track>> GetTracks()
    {
        return await _db.Tracks.Find(_ => true).ToListAsync();
    }

    public Track CreateTrack(TrackDto trackDto)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Artist>> GetArtists()
    {
        return await _db.Artists.Find(_ => true).ToListAsync();
    }

    public Track CreateArtist(ArtistDto artistDto)
    {
        throw new NotImplementedException();
    }

    public Track CreateTrack()
    {
        throw new NotImplementedException();
    }
}