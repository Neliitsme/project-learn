using System.ComponentModel.DataAnnotations;

namespace devtools_proj.Settings;

public interface IGeneralSettings
{
    [Required(AllowEmptyStrings = false)] public string ProjectName { get; set; }
}