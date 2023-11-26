namespace devtools_proj.Settings;

public interface IMongoSettings
{
    public string? ConnectionUri { get; set; }

    public string? DatabaseName { get; set; }
}