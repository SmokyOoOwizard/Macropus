using Autofac;
using Macropus.CoolStuff;
using Macropus.Project.Raw;
using Odin.Context;

namespace Macropus.Project.Instance.Impl;

internal sealed class ProjectInstance : IProjectInstance, IMakeRef<IProjectInstance>
{
	public IProjectInformation ProjectInformation => rawProject.ProjectInformation;

	private readonly ILifetimeScope scope;
	private readonly IRawProject rawProject;
	private IOdinContext? odinContext;

	private bool disposed;

	public ProjectInstance(ILifetimeScope scope, IRawProject rawProject)
	{
		this.scope = scope;
		this.rawProject = rawProject;

	}

	public async Task InitializeAsync(CancellationToken ctx = default)
	{
		var odinBuilder = Odin.Odin.CreateContextBuilder(scope);
		odinContext = odinBuilder.Build();

		await odinContext.InitAsync(ctx);
		await odinContext.StartAsync(ctx);
	}

	private async Task TerminateAsync()
	{
		if (odinContext != null)
			await odinContext.StopAsync();
	}

	public void Dispose()
	{
		if(disposed)
			return;
		disposed = true;

		TerminateAsync().ConfigureAwait(false).GetAwaiter().GetResult();

		scope.Dispose();
	}

	~ProjectInstance()
	{
		Dispose();
	}

	public IProjectInstance MakeRef()
	{
		// TODO
		return this;
	}
}