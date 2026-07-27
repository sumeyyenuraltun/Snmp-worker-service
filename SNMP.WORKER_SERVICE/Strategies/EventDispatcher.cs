using Snmp.EventWorker.BackgroundServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Strategies
{
    public class EventDispatcher: IEventDispatcher
    {
        private readonly Dictionary<string, IEventStrategy> _strategies;
        private readonly ILogger<EventDispatcher> _logger;

        public EventDispatcher(IEnumerable<IEventStrategy> strategies, ILogger<EventDispatcher> logger)
        {
            _logger = logger;

            var duplicateEventTypes = strategies.GroupBy(s => s.EventType)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

            if (duplicateEventTypes.Count != 0)
            {
                throw new InvalidOperationException(
                    $"Duplicate EventType(s) found in strategies: {string.Join(", ", duplicateEventTypes)}"
                );
            }
            _strategies = strategies.ToDictionary(s => s.EventType, s => s);
        }

        public async Task DispatchAsync(EventMessage eventMessage, CancellationToken cancellationToken)
        {
            if (_strategies.TryGetValue(eventMessage.EventType, out var strategy))
            {
                await strategy.HandleEventAsync(eventMessage, cancellationToken);
            }

            _logger.LogWarning(
            "No strategy found for event type {EventType}",
            eventMessage.EventType);
        }
    }
}
