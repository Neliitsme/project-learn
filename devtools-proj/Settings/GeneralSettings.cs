using System.ComponentModel.DataAnnotations;

namespace devtools_proj.Settings;

public class GeneralSettings : IGeneralSettings
{
    [Required(AllowEmptyStrings = false)] public required string ProjectName { get; set; }
}