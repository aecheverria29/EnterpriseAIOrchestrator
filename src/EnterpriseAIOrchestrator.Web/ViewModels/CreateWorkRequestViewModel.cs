using System.ComponentModel.DataAnnotations;

namespace EnterpriseAIOrchestrator.Web.ViewModels;

public sealed class CreateWorkRequestViewModel
{
    [Required]
    [StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    [Display(Name = "Requested by")]
    public string RequestedBy { get; set; } = string.Empty;

    [Required]
    public string Priority { get; set; } = "medium";

    [StringLength(300)]
    public string? Tags { get; set; }
}
