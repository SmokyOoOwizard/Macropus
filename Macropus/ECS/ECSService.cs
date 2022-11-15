using Delogger;
using Delogger.Scope;
using Macropus.ECS.Entity.Context;
using Macropus.ECS.Systems;
using Macropus.Service;

namespace Macropus.ECS;

public class ECSService : IService
{
	private readonly EntitiesContext context;
	private readonly IEnumerable<ASystem> systems;
	private readonly IDScope scopeLogger;

	private readonly SystemsExecutor executor;

	private readonly Thread thread;
	private volatile bool running;

	public EServiceStatus Status { get; private set; } = EServiceStatus.ReadyToStart;

	public ECSService(EntitiesContext context, IEnumerable<ASystem> systems, IDScope scopeLogger)
	{
		this.context = context ?? throw new ArgumentNullException(nameof(context));
		this.systems = systems ?? throw new ArgumentNullException(nameof(systems));
		this.scopeLogger = scopeLogger;

		executor = new SystemsExecutor(systems as ASystem[] ?? systems.ToArray());

		thread = new Thread(ThreadJob);
	}

	public void Start()
	{
		running = true;
		Status = EServiceStatus.Starting;
		thread.Start();
	}

	public void Stop()
	{
		running = false;
		Status = EServiceStatus.Termination;
		thread.Join();
	}

	private void ThreadJob()
	{
		Status = EServiceStatus.Started;

		using var logger = scopeLogger.CreateLogger();

		try
		{
			while (running)
			{
				using var perfMonitor = scopeLogger.CreatePerfMonitor();

				executor.Execute(context);
				context.SaveChanges();

				Thread.Sleep(10);
			}
		}
		catch (Exception e)
		{
			logger.LogException(e);
			Status = EServiceStatus.Error;
			return;
		}

		Status = EServiceStatus.Terminated;
	}
}