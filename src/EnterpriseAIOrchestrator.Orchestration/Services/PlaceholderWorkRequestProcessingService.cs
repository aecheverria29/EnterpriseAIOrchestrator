using EnterpriseAIOrchestrator.Application.Abstractions;
using EnterpriseAIOrchestrator.Application.Common;
using EnterpriseAIOrchestrator.Application.UseCases.ProcessWorkRequest;
using EnterpriseAIOrchestrator.Domain.Aggregates;

namespace EnterpriseAIOrchestrator.Orchestration.Services;

public sealed class PlaceholderWorkRequestProcessingService : IWorkRequestProcessingService
{
    private readonly WorkRequestProcessingPipeline _pipeline;
    private readonly IWorkflowRunStore _workflowRunStore;

    public PlaceholderWorkRequestProcessingService(
        WorkRequestProcessingPipeline pipeline,
        IWorkflowRunStore workflowRunStore)
    {
        _pipeline = pipeline;
        _workflowRunStore = workflowRunStore;
    }

    public async Task<ProcessWorkRequestResult> ProcessAsync(
        ProcessWorkRequestCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        cancellationToken.ThrowIfCancellationRequested();

        var priority = RequestPriorityParser.Parse(command.Priority);

        var workRequest = new WorkRequest(
            command.Title,
            command.Description,
            command.Department,
            command.RequestedBy,
            priority,
            command.Tags);

        var result = await _pipeline.ExecuteAsync(workRequest, cancellationToken);
        await _workflowRunStore.SaveAsync(result, cancellationToken);

        return result;
    }
}
