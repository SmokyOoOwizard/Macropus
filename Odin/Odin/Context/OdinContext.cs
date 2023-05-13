using Nito.AsyncEx;
using Odin.ECS;

namespace Odin.Context;

internal sealed class OdinContext : IOdinContext
{
	public EContextStatus Status { get; private set; } = EContextStatus.UNKNOWN;

	private readonly AsyncAutoResetEvent threadStartEvent;
	private readonly AsyncAutoResetEvent threadEndEvent;

	private readonly IECSContextsInternal? ecsContexts;

	private readonly AsyncContextThread contextThread = new();

	public OdinContext()
	{
		threadStartEvent = new(false);
		threadEndEvent = new(false);
	}

	public OdinContext(IECSContextsInternal? ecsContexts) : this()
	{
		this.ecsContexts = ecsContexts;
	}

	public async Task InitAsync(CancellationToken ctx = default)
	{
		if (Status != EContextStatus.UNKNOWN)
			throw new(); // TODO

		Status = EContextStatus.Initialize;
		// TODO
	}

	public async Task StartAsync(CancellationToken ctx = default)
	{
		if (Status != EContextStatus.Initialize)
			throw new(); // TODO

		Status = EContextStatus.Run;

		_ = contextThread.Factory.Run(ThreadJob);

		await threadStartEvent.WaitAsync(ctx);
	}

	public async Task StopAsync()
	{
		if (Status != EContextStatus.Run)
			throw new(); // TODO

		Status = EContextStatus.Stop;

		await threadEndEvent.WaitAsync();
	}

	private async Task ThreadJob()
	{
		threadStartEvent.Set();

		try
		{
			while (true)
			{
				if (Status != EContextStatus.Run)
					break;

				if (ecsContexts != null)
					await ecsContexts.TickAsync();
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e); // TODO log
			Status = EContextStatus.UNKNOWN;
		}

		threadEndEvent.Set();
	}

	public void Dispose()
	{
		Status = EContextStatus.Dispose;

		contextThread.Dispose();
		// TODO
	}

	public async ValueTask DisposeAsync()
	{
		Status = EContextStatus.Dispose;

		contextThread.Dispose();
		// TODO
	}
}