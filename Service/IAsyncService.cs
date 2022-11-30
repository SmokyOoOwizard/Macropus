namespace Macropus.Service;

public interface IAsyncService : IServiceBase
{
	Task StartAsync(CancellationToken cancellationToken = default);
	Task StopAsync();
}