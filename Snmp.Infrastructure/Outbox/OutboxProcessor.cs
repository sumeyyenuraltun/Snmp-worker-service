using Microsoft.Extensions.Logging;
using Snmp.DataAccess.Abstract;
using Snmp.Entity.Abstract;
using Snmp.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snmp.Infrastructure.Outbox
{
    public class OutboxProcessor : IOutboxProcessor
    {
        private readonly IOutboxMessageDAL _outboxMessageDAL;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<OutboxProcessor> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public OutboxProcessor(IOutboxMessageDAL outboxMessageDAL, IEventPublisher eventPublisher, ILogger<OutboxProcessor> logger, IUnitOfWork unitOfWork  )
        {
            _outboxMessageDAL = outboxMessageDAL;
            _eventPublisher = eventPublisher;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task ProcessAsync(CancellationToken cancellationToken = default)
        {
            var messages = await _outboxMessageDAL.GetAllAsync(x=> !x.IsProcessed);

            foreach( var message in messages)
            {
                try
                {
                    var eventType = Type.GetType(message.EventType);

                    if(eventType == null)
                    {
                        _logger.LogWarning("Event type {EventType} not found for outbox message with ID {MessageId}", message.EventType, message.Id);
                        continue;
                    }

                    var @event = JsonSerializer.Deserialize(message.Payload, eventType);

                    if(@event is not IEvent domaintEvent) 
                    {
                        _logger.LogWarning("Payload couldn't be deserialized to IEvent for outbox message with ID {MessageId}", message.Id);
                        continue;
                    }

                    await _eventPublisher.PublishAsync(domaintEvent, cancellationToken);

                    message.IsProcessed = true;
                    message.ProcessedOn = DateTime.UtcNow;

                    await _outboxMessageDAL.UpdateAsync(message);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing outbox message with ID {MessageId}", message.Id);
                }

            }
        }
    }
}
