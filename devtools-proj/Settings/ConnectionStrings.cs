using System.ComponentModel.DataAnnotations;

namespace devtools_proj.Settings;

public class ConnectionStrings : IConnectionStrings
{
    [Required] public required string MongoUri { get; set; }

    [Required(AllowEmptyStrings = false)] public required string MongoDatabaseName { get; set; }

    [Required] public required string LokiUri { get; set; }
}