using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace RepoGateway.HealthChecks
{
    public class RabbitMqHealthCheck : IHealthCheck
    {
        private readonly string _host;

        public RabbitMqHealthCheck(string host)
        {
            _host = host;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var factory = new ConnectionFactory { HostName = _host };
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();
                return HealthCheckResult.Healthy("RabbitMQ is OK");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("RabbitMQ is unhealthy", ex);
            }
        }
    }
}
