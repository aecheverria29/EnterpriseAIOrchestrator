using EnterpriseAIOrchestrator.Application.UseCases.ProcessWorkRequest;

namespace EnterpriseAIOrchestrator.Application.Abstractions;

public interface IWorkRequestProcessingService
{
    Task<ProcessWorkRequestResult> ProcessAsync(
        ProcessWorkRequestCommand command,
        CancellationToken cancellationToken = default);
}
