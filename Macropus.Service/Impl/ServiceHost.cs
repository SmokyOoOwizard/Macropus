using Delogger;
using Delogger.Scope;
using Delogger.Scope.Perf;

namespace Macropus.Service.Impl;

public class ServiceHost
{
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

			var serviceName = service.GetType().Name;

			using var logger = scopeLogger.CreatePerfMonitor(new PerfMonitorCreateOptions()
				{ Tags = new[] { serviceName, "Start" } });


			logger.AddAttachment("Service", serviceName);

			try
			{
				if (service is IAsyncService asyncService)
					await asyncService.StartAsync(cancellationToken);
				else if (service is IService justService)
					justService.Start();
				else
				{
					logger.Log("Service: not implement any of the interfaces: {1}, {2}",
						new[] { serviceName, "Start", "Warning" },
						new Object[] { nameof(IService), nameof(IAsyncService) });
				}
			}
			catch (Exception e)
			{
				logger.LogException(e);
				// TODO
				throw new Exception();
			}
		}
	}


	public async Task StopAsync(CancellationToken cancellationToken)
	{
		if (services.Length == 0)
			return;

		foreach (var service in services.Reverse())
		{
			var serviceName = service.GetType().Name;

			using var logger = scopeLogger.CreatePerfMonitor(new PerfMonitorCreateOptions()
				{ Tags = new[] { serviceName, "Terminate" } });

			logger.AddAttachment("Service", serviceName);

			try
			{
				if (service is IAsyncService asyncService)
					await asyncService.StopAsync();
				else if (service is IService justService)
					justService.Stop();
				else
				{
					logger.Log("Service not implement any of the interfaces: {1}, {2}",
						new[] { serviceName, "Terminate", "Warning" },
						new Object[] { nameof(IService), nameof(IAsyncService) });
				}
			}
			catch (Exception e)
			{
				logger.LogException(e);
			}
		}

		// TODO delogger error. write thread end before all logs be write
		await Task.Delay(1000);
	}
}