using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RepoGateway.Hubs;
using RepoGateway.Models;
using System.Text;
using System.Text.Json;

namespace RepoGateway.Core.Workers
{
    public class ReportConsumer : BackgroundService
    {
        private readonly IHubContext<ReportHub> _hub;
        private readonly IConfiguration _config;
        private IConnection? _connection;
        private IChannel? _channel;
        private const string ExchangeName = "analysis.ready";

        public ReportConsumer(IHubContext<ReportHub> hub, IConfiguration configuration)
        {
            _hub = hub;
            _config = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            var factory = new ConnectionFactory { HostName = _config["RabbitMQ:Host"] ?? "localhost" };
            _connection = await factory.CreateConnectionAsync(ct);
            _channel = await _connection.CreateChannelAsync(cancellationToken: ct);

            await _channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Fanout, durable: true, cancellationToken: ct);
            var queueName = await _channel.QueueDeclareAsync(
                queue: "",
                durable: false,
                exclusive: true,
                autoDelete: true,
                cancellationToken: ct
            );

            await _channel.QueueBindAsync(queue: queueName.QueueName, exchange: ExchangeName, routingKey: "", cancellationToken: ct);
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var report = JsonSerializer.Deserialize<RepoAnalysis>(json);

                await _hub.Clients.User(report.UserId).SendAsync("ReceiveReport", report);

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            await _channel.BasicConsumeAsync(queue: queueName.QueueName, autoAck: false, consumer: consumer, cancellationToken: ct);
        }

        public async Task AckMessage(BasicDeliverEventArgs ea)
        {
            if (_channel != null)
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
        }


        public async Task NackMessage(BasicDeliverEventArgs ea, bool requeue)
        {
            if (_channel != null)
                await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue);
        }
    }
}
