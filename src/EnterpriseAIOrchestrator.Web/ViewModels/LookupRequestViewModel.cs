using System.ComponentModel.DataAnnotations;

namespace EnterpriseAIOrchestrator.Web.ViewModels;

public sealed class LookupRequestViewModel
{
    [Required]
    [Display(Name = "Run ID")]
    public Guid RunId { get; set; }
}
