using RepoGateway.Models;

namespace RepoGateway.Core.Interfaces
{
    public interface IEventPublisher
    {
        public Task PublishRepoFavoritedAsync(RepoFavoritedEvent evt);

        public Task StartAsync(CancellationToken cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken);
    }
}
