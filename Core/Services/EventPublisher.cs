using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using RepoGateway.Core.Interfaces;
using RepoGateway.Models;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace RepoGateway.Core.Services
{
    public class EventPublisher : IHostedService, IEventPublisher
    {
        private IConnection _connection;
        private IChannel _channel;
        private IConfiguration _configuration;
        private const string ExchangeName = "repo.favorited";

        public EventPublisher(IConfiguration config)
        {
            _configuration = config;
        }

        public async Task PublishRepoFavoritedAsync(RepoFavoritedEvent evt)
        {
            if (_channel == null || _connection == null || !_channel.IsOpen || !_connection.IsOpen)
                throw new InvalidOperationException("RabbitMQ channel/connection is not open.");

            var json = JsonSerializer.Serialize(evt);
            var body = Encoding.UTF8.GetBytes(json);

            var props = new BasicProperties() {  Persistent = true };

            await _channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: "",
                mandatory: false,
                basicProperties: props,
                body: body
            );

            Console.WriteLine($"[RabbitMQ] Published repo.favorited: {evt.RepoId}");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"] ?? "localhost"
            };
            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Fanout, true, cancellationToken: cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel.IsOpen)
                await _channel.CloseAsync(cancellationToken);
            if (_connection.IsOpen)
                await _connection.CloseAsync(cancellationToken);
        }
    }
}
