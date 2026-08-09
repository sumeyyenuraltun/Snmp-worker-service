using Lextm.SharpSnmpLib;
using Snmp.EventWorker.Snmp.Clients;
using Snmp.EventWorker.Snmp.Models;
using SNMP.ENTITY.Enums;


namespace Snmp.EventWorker.Snmp.Services
{
    public class SnmpService : ISnmpService
    {
        private readonly IReadOnlyDictionary<SnmpVersion, ISnmpClient> _clients;
        private readonly ILogger<SnmpService> _logger;

        public SnmpService(IEnumerable<ISnmpClient> clients, ILogger<SnmpService> logger)
        {
            _clients = clients.ToDictionary(c => c.Version);
            _logger = logger;
        }

        public Task<string?> GetAsync(SnmpRequest request, CancellationToken cancellationToken = default) =>
            ExecuteAsync("GET", request, client => client.GetAsync(request, cancellationToken));

        public Task<string?> GetNextAsync(SnmpRequest request, CancellationToken cancellationToken = default) =>
            ExecuteAsync("GETNEXT", request, client => client.GetNextAsync(request, cancellationToken));

        public Task SetAsync(SnmpRequest request, CancellationToken cancellationToken = default) =>
            ExecuteAsync("SET", request, async client =>
            {
                await client.SetAsync(request, cancellationToken);
                return string.Empty;
            });

        public Task<IList<Variable>> WalkAsync(SnmpRequest request, CancellationToken cancellationToken = default) =>
            ExecuteAsync("WALK", request, client => client.WalkAsync(request, cancellationToken));

        private async Task<T> ExecuteAsync<T>(string operation, SnmpRequest request, Func<ISnmpClient, Task<T>> action)
        {
            var client = Resolve(request);

            try
            {
                return await action(client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SNMP {Operation} failed. IP:{Ip}, OID:{Oid}",
                    operation, request.IpAddress, request.Oid);

                throw;
            }
        }

        private ISnmpClient Resolve(SnmpRequest request)
        {
            if (_clients.TryGetValue(request.Credential.Version, out var client))
                return client;

            throw new NotSupportedException(
                $"SNMP version '{request.Credential.Version}' is not supported.");
        }
    }
}
