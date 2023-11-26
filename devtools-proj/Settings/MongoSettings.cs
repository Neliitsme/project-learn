namespace devtools_proj.Settings;

public class MongoSettings : IMongoSettings
{
    public string? ConnectionUri { get; set; }

    public string? DatabaseName { get; set; }
}