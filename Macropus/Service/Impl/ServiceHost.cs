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

		foreach (var service in services)
		{
			switch (service.Status)
			{
				case EServiceStatus.Error:
				case EServiceStatus.Started:
				case EServiceStatus.Starting:
				case EServiceStatus.Terminated:
				case EServiceStatus.Termination:
					continue;
			}

			using var logger = scopeLogger.CreatePerfMonitor(new PerfMonitorCreateOptions());

			var serviceName = service.GetType().Name;

			logger.Log("Try start service: {0}",
				new[] { serviceName, "Start" }, new Object[] { serviceName });

			if (service is IAsyncService asyncService)
				await asyncService.StartAsync(cancellationToken);
			else if (service is IService justService)
				justService.Start();
			else
			{
				logger.Log("Service: {0} not implement any of the interfaces: {1}, {2}",
					new[] { serviceName, "Start", "Warning" },
					new Object[] { serviceName, nameof(IService), nameof(IAsyncService) });
			}
		}
	}


	public async Task StopAsync(CancellationToken cancellationToken)
	{
		if (services.Length == 0)
			return;

		foreach (var service in services)
		{
			using var logger = scopeLogger.CreatePerfMonitor(new PerfMonitorCreateOptions());

			var serviceName = service.GetType().Name;

			logger.Log("Try terminate service: {0}",
				new[] { serviceName, "Terminate" }, new Object[] { serviceName });

			if (service is IAsyncService asyncService)
				await asyncService.StopAsync();
			else if (service is IService justService)
				justService.Stop();
			else
			{
				logger.Log("Service: {0} not implement any of the interfaces: {1}, {2}",
					new[] { serviceName, "Terminate", "Warning" },
					new Object[] { serviceName, nameof(IService), nameof(IAsyncService) });
			}
		}
	}
}