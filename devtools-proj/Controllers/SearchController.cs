using devtools_proj.Persistence.Entities;
using devtools_proj.Services;
using Microsoft.AspNetCore.Mvc;

namespace devtools_proj.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ILogger<SearchController> _logger;

    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService,
        ILogger<SearchController> logger)
    {
        _logger = logger;
        _searchService = searchService;
    }

    [HttpGet("tracks")]
    public async Task<IEnumerable<Track>> GetTracks()
    {
        return await _searchService.GetTracks();
    }

    [HttpPost("tracks")]
    public async Task<Track> CreateTrack()
    {
        throw new NotImplementedException();
        // return await _searchService.CreateTrack();
    }

    [HttpGet("artists")]
    public async Task<IEnumerable<Artist>> GetArtists()
    {
        return await _searchService.GetArtists();
    }

    [HttpPost("artists")]
    public async Task<Artist> CreateArtist()
    {
        throw new NotImplementedException();
        // return await _searchService.CreateArtist();
    }
}