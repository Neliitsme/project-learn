using devtools_proj.DTOs;
using devtools_proj.Metrics.ReporterInterfaces;
using devtools_proj.Persistence;
using devtools_proj.Persistence.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace devtools_proj.Services;

public class SearchService : ISearchService
{
    private readonly IDbContext _db;

    private readonly ILogger<ISearchService> _logger;

    private readonly ITracksMetricsReporter _metricsReporter;

    public SearchService(IDbContext db, ITracksMetricsReporter metricsReporter, ILogger<ISearchService> logger)
    {
        _db = db;
        _logger = logger;
        _metricsReporter = metricsReporter;
    }

    public async Task<IEnumerable<TrackDto>> GetTracks()
    {
        var tracks = await _db.Tracks.Find(_ => true).ToListAsync();
        _logger.LogInformation($"Fetched all {nameof(Track)}s.");
        return tracks.Select(t => new TrackDto(t));
    }

    public async Task<TrackDto> GetTrack(string trackId)
    {
        var track = await _db.Tracks.Find(t => t.Id == ObjectId.Parse(trackId)).FirstOrDefaultAsync();

        if (track is null)
        {
            _logger.LogError($"{nameof(Track)} with id {trackId} was not found.");
            throw new ArgumentException();
        }

        _logger.LogInformation($"Fetched a {nameof(Track)} with id {trackId}");
        return new TrackDto(track);
    }

    public async Task<TrackDto> CreateTrack(CreateTrackDto trackDto)
    {
        var track = new Track(trackDto);
        await _db.Tracks.InsertOneAsync(track);

        _logger.LogInformation($"Created {nameof(Track)} {track.Id.ToString()}.");
        _metricsReporter.IncrementGauge();
        return new TrackDto(track);
    }

    public async Task DeleteTrack(string trackId)
    {
        var result = await _db.Tracks.DeleteOneAsync(t => t.Id == ObjectId.Parse(trackId));
        if (result.DeletedCount == 0)
        {
            _logger.LogError($"{nameof(Track)} with id {trackId} was not found.");
            throw new ArgumentException();
        }

        _logger.LogInformation($"{nameof(Track)} has been removed.");
        _metricsReporter.DecrementGauge();
    }

    public async Task UpdateTrack(TrackDto trackDto)
    {
        var updateBuilder = Builders<Track>.Update;
        var update = updateBuilder.Set(t => t.Name, trackDto.Name);

        if (trackDto.Author is not null)
        {
            update = update
                .Set(t => t.Author, trackDto.Author);
        }

        if (trackDto.Artists is not null)
        {
            update = update
                .Set(t => t.Artists, trackDto.Artists);
        }

        if (trackDto.Genres is not null)
        {
            update = update
                .Set(t => t.Genres, trackDto.Genres);
        }

        if (trackDto.Length is not null)
        {
            update = update
                .Set(t => t.Length, trackDto.Length);
        }

        var result = await _db.Tracks.UpdateOneAsync(t => t.Id == ObjectId.Parse(trackDto.Id), update);

        if (result.MatchedCount == 0)
        {
            _logger.LogError($"{nameof(Track)} with id {trackDto.Id} was not found.");
            throw new ArgumentException();
        }

        _logger.LogInformation($"{nameof(Track)} {trackDto.Id} has been updated.");
    }
}