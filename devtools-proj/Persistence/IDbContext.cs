using devtools_proj.Persistence.Entities;
using MongoDB.Driver;

namespace devtools_proj.Persistence;

public interface IDbContext
{
    public IMongoCollection<Track> Tracks { get; }

    public IMongoCollection<Artist> Artists { get; }
}