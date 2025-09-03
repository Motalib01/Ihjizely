using Ihjezly.Infrastructure;
using Ihjezly.Infrastructure.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;

public class ProcessOutboxMessagesJob : IJob
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;

    public ProcessOutboxMessagesJob(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ProcessOutboxMessagesJob> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Processing Outbox messages...");

        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        using var transaction = await dbContext.Database.BeginTransactionAsync();

        var messages = await dbContext.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .ToListAsync();

        foreach (var message in messages)
        {
            try
            {
                _logger.LogInformation("Dispatching OutboxMessage ID: {Id}, Type: {Type}", message.Id, message.Type);

                var domainEvent = JsonConvert.DeserializeObject(
                    message.Content,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }
                ) as INotification; // Domain events must implement INotification

                if (domainEvent != null)
                {
                    await mediator.Publish(domainEvent);
                    message.MarkAsProcessed();
                }
                else
                {
                    message.Error = "Deserialized domain event is null or invalid.";
                    _logger.LogWarning("Failed to deserialize domain event: {Content}", message.Content);
                }
            }
            catch (Exception ex)
            {
                message.Error = ex.ToString();
                _logger.LogError(ex, "Failed to dispatch outbox message with ID: {Id}", message.Id);
            }
        }

        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}
