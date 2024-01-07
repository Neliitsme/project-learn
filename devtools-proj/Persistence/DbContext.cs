using devtools_proj.Persistence.Entities;
using devtools_proj.Persistence.Entities.EntityInterfaces;
using devtools_proj.Settings;
using MongoDB.Driver;

namespace devtools_proj.Persistence;

public class DbContext : IDbContext
{
    private readonly IMongoDatabase _db;

    public DbContext(IMongoClient mongoClient, IConnectionStrings settings)
    {
        _db = mongoClient.GetDatabase(settings.MongoDatabaseName);

        SetupEntities();
    }

    public IMongoCollection<Track> Tracks { get; private set; } = null!;

    public IMongoCollection<Artist> Artists { get; private set; } = null!;

    private IMongoCollection<T> GetCollectionFromDb<T>() where T : IHasId
    {
        return _db.GetCollection<T>($"{typeof(T).Name}s");
    }

    private void SetupEntities()
    {
        SetupTracks();
        SetupArtists();
    }

    private void SetupTracks()
    {
        Tracks = GetCollectionFromDb<Track>();
    }

    private async void SetupArtists()
    {
        Artists = GetCollectionFromDb<Artist>();

        var indexKeys = Builders<Artist>.IndexKeys.Text(m => m.Alias);
        var indexOptions = new CreateIndexOptions { Unique = true };

        await Artists.Indexes.CreateOneAsync(new CreateIndexModel<Artist>(indexKeys, indexOptions));
    }
}