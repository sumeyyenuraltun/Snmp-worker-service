using Microsoft.Extensions.Logging;
using Snmp.Business.Abstract;
using Snmp.DataAccess.Abstract;
using Snmp.Entity.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Infrastructure.Outbox
{
    public class OutboxPublisher : IOutboxPublisher
    {
        private readonly IOutboxMessageDAL _outboxMessageDAL;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<OutboxPublisher> _logger;

        public OutboxPublisher(IOutboxMessageDAL outboxMessageDAL, IEventPublisher eventPublisher, ILogger<OutboxPublisher> logger)
        {
            _outboxMessageDAL = outboxMessageDAL;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task PublishPendingMessagesAsync(CancellationToken cancellationToken = default)
        {
            var messages = await _outboxMessageDAL.GetAllAsync(x => !x.IsProcessed);

            foreach (var message in messages) 
            {
                try
                {
                    message.IsProcessed = true;
                    await _outboxMessageDAL.UpdateAsync(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing outbox message.");
                }
            }
        }
    }
}
