using Delogger.Scope;
using Delogger.Scope.Perf;

namespace Macropus.Service.Impl;

public class ServiceHost
{
	private static readonly string[] StartServiceTags =
	{
		nameof(ServiceHost), "Start"
	};

	private static readonly string[] StopServiceTags =
	{
		nameof(ServiceHost), "Terminate"
	};

	private readonly IServiceBase[] services;

	private readonly IDScope scopeLogger;

	public ServiceHost(IEnumerable<IServiceBase> services, IDScope scopeLogger)
	{
		this.scopeLogger = scopeLogger;
		this.services = services.Where(s => s.Status == EServiceStatus.ReadyToStart).ToArray();
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		if (services.Length == 0)
			return;

		using var logger = scopeLogger.CreatePerfMonitor(new PerfMonitorCreateOptions());

		logger.Log("Starting services: {0}", StartServiceTags,
			services.Select(s => (object)s.GetType().Name).ToArray());

		foreach (var service in services)
		{
			using var startupLogger = scopeLogger.CreatePerfMonitor(new PerfMonitorCreateOptions());

			var serviceName = service.GetType().Name;

			startupLogger.Log("Try start service: {0}",
				new[] { serviceName, "Start" }, new Object[] { serviceName });

			if (service is IAsyncService asyncService)
				await asyncService.StartAsync(cancellationToken);
			else if (service is IService justService)
				justService.Start();
			else
			{
				startupLogger.Log("Service: {0} not implement any of the interfaces: {1}, {2}",
					new[] { serviceName, "Start", "Warning" },
					new Object[] { serviceName, nameof(IService), nameof(IAsyncService) });
			}
		}
	}


	public async Task StopAsync(CancellationToken cancellationToken)
	{
		if (services.Length == 0)
			return;

		using var logger = scopeLogger.CreatePerfMonitor(new PerfMonitorCreateOptions());

		logger.Log("Termination services: {0}", StopServiceTags,
			services.Select(s => (object)s.GetType().Name).ToArray());

		foreach (var service in services)
		{
			using var shutdownLogger = scopeLogger.CreatePerfMonitor(new PerfMonitorCreateOptions());

			var serviceName = service.GetType().Name;

			shutdownLogger.Log("Try terminate service: {0}",
				new[] { serviceName, "Terminate" }, new Object[] { serviceName });

			if (service is IAsyncService asyncService)
				await asyncService.StopAsync();
			else if (service is IService justService)
				justService.Stop();
			else
			{
				shutdownLogger.Log("Service: {0} not implement any of the interfaces: {1}, {2}",
					new[] { serviceName, "Terminate", "Warning" },
					new Object[] { serviceName, nameof(IService), nameof(IAsyncService) });
			}
		}
	}
}