using RepoGateway.Core.Interfaces;

namespace RepoGateway.Core.Services
{
    public class AppInitializer : IHostedService
    {
        private readonly IEventPublisher _rabbitProducer;
        
        public AppInitializer(IEventPublisher rabbitProducer)
        {
            _rabbitProducer = rabbitProducer;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _rabbitProducer.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _rabbitProducer.StopAsync(cancellationToken);
        }
    }
}
