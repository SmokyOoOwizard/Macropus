using Microsoft.Extensions.Hosting;

namespace Macropus.Service.Impl;

public class MainServiceHost : IHostedService
{
	private readonly ServiceHost host;

	public MainServiceHost(ServiceHost host)
	{
		this.host = host;
	}

	public Task StartAsync(CancellationToken cancellationToken)
		=> host.StartAsync(cancellationToken);

	public Task StopAsync(CancellationToken cancellationToken)
		=> host.StopAsync(cancellationToken);
}