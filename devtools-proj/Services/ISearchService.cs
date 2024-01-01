using devtools_proj.DTOs;

namespace devtools_proj.Services;

public interface ISearchService
{
    public Task<IEnumerable<TrackDto>> GetTracks();

    public Task<TrackDto> GetTrack(string trackId);

    public Task<TrackDto> CreateTrack(CreateTrackDto trackDto);

    public Task DeleteTrack(string trackId);

    public Task UpdateTrack(TrackDto trackDto);
}