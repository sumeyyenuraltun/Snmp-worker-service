using Microsoft.AspNetCore.Http;
using Snmp.Business.Abstract.Outbox;
using Snmp.DataAccess.Abstract;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snmp.Business.Concrete.Outbox
{
    public class OutboxService : IOutboxService
    {
        private readonly IOutboxMessageDAL _outboxMessageDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OutboxService(IOutboxMessageDAL outboxMessageDAL, IHttpContextAccessor httpContextAccessor)
        {
            _outboxMessageDAL = outboxMessageDAL;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AddMessageAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
        {
            var message = new OutboxMessage
            {
                EventType = typeof(T).AssemblyQualifiedName!,
                Payload = JsonSerializer.Serialize(@event),
                CorrelationId = _httpContextAccessor.HttpContext?.Items["X-Correlation-ID"]?.ToString(),
                OccurredOn = DateTime.UtcNow,
                IsProcessed = false
            };

            await _outboxMessageDAL.AddAsync(message, cancellationToken);
        }
    }
}
