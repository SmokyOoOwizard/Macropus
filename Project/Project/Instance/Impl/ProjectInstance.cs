using Autofac;
using Macropus.CoolStuff;
using Macropus.Project.Raw;

namespace Macropus.Project.Instance.Impl;

internal sealed class ProjectInstance : IProjectInstance, IMakeRef<IProjectInstance>
{
	public IProjectInformation ProjectInformation => rawProject.ProjectInformation;

	private readonly ILifetimeScope scope;
	private readonly IRawProject rawProject;

	public ProjectInstance(ILifetimeScope scope, IRawProject rawProject)
	{
		this.scope = scope;
		this.rawProject = rawProject;
	}

	public async Task InitializeAsync(CancellationToken cancellationToken = default)
	{

	}

	private async Task TerminateAsync()
	{

	}

	public void Dispose()
	{
		TerminateAsync().ConfigureAwait(false).GetAwaiter().GetResult();

		scope.Dispose();
	}

	public IProjectInstance MakeRef()
	{
		// TODO
		return this;
	}
}