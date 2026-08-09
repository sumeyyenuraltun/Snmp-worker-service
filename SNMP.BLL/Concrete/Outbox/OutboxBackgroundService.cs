using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Snmp.Business.Abstract.Outbox;

namespace Snmp.Business.Concrete.Outbox
{
    public class OutboxBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<OutboxBackgroundService> _logger;
        public OutboxBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<OutboxBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox background service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();

                    var outboxProcessor =
                        scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();

                    await outboxProcessor.ProcessAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing the outbox messages.");
                }
                await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
            }

            _logger.LogInformation("Outbox background service is stopping.");
        }
    }
}
