using devtools_proj.DTOs;
using devtools_proj.Services;
using Microsoft.AspNetCore.Mvc;

namespace devtools_proj.Controllers;

[ApiController]
[Route("api/[controller]/tracks")]
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TrackDto>>> GetTracks()
    {
        IEnumerable<TrackDto> result;
        try
        {
            result = await _searchService.GetTracks();
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok(result);
    }

    [HttpGet("{trackId}")]
    public async Task<ActionResult<TrackDto>> GetTrack(string trackId)
    {
        TrackDto result;
        try
        {
            result = await _searchService.GetTrack(trackId);
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return NotFound($"Track with id {trackId} was not found.");
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TrackDto>> CreateTrack([FromBody] CreateTrackDto trackDto)
    {
        TrackDto result;
        try
        {
            result = await _searchService.CreateTrack(trackDto);
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return BadRequest();
        }

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpDelete("{trackId}")]
    public async Task<ActionResult> DeleteTrack(string trackId)
    {
        try
        {
            await _searchService.DeleteTrack(trackId);
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return NotFound($"Track with id {trackId} was not found.");
        }

        return Ok();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateTrack([FromBody] TrackDto trackDto)
    {
        try
        {
            await _searchService.UpdateTrack(trackDto);
        }
        catch (ArgumentException)
        {
            return NotFound($"Track with id {trackDto.Id} was not found.");
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return BadRequest();
        }

        return Ok();
    }
}