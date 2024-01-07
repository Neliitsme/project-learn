using System.ComponentModel.DataAnnotations;

namespace devtools_proj.Settings;

public interface IConnectionStrings
{
    [Required] public string MongoUri { get; set; }

    [Required(AllowEmptyStrings = false)] public string MongoDatabaseName { get; set; }

    [Required] public string LokiUri { get; set; }
}