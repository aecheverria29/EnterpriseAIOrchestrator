using System.ComponentModel.DataAnnotations;

namespace EnterpriseAIOrchestrator.Web.ViewModels;

public sealed class ReviewDecisionViewModel
{
    [Required]
    [StringLength(120)]
    [Display(Name = "Performed by")]
    public string PerformedBy { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Comment { get; set; }
}
