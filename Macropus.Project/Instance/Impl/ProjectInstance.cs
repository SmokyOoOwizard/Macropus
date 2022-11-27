using System.Reactive.Linq;
using System.Reactive.Subjects;
using Autofac;
using Delogger.Scope;
using Delogger.Scope.Perf;
using Macropus.CoolStuff;
using Macropus.Interfaces.Project;
using Macropus.Project.Raw;

namespace Macropus.Project.Instance.Impl;

internal sealed class ProjectInstance : IProjectInstance, IMakeRef<IProjectInstance>
{
	private readonly ILifetimeScope scope;
	private readonly IRawProject rawProject;
	private readonly IDScope dScope;
	private readonly BehaviorSubject<EInstanceState> stateSubject;
	
	public IObservable<EInstanceState> State => stateSubject.AsObservable();
	public IProjectInformation ProjectInformation => rawProject.ProjectInformation;

	public ProjectInstance(ILifetimeScope scope, IRawProject rawProject)
	{
		this.scope = scope;
		this.rawProject = rawProject;

		dScope = scope.Resolve<IDScope>();

		stateSubject = new(EInstanceState.NotInit);
	}

	public async Task InitializeAsync(CancellationToken cancellationToken = default)
	{
		if (stateSubject.Value != EInstanceState.NotInit)
			return;

		stateSubject.OnNext(EInstanceState.Initialization);

		using (var perfScope = dScope.CreatePerfMonitor(new PerfMonitorCreateOptions()
			       { Tags = new[] { nameof(ProjectInstance), nameof(InitializeAsync) } }))
		{
			perfScope.AddAttachment("Project Id", rawProject.ProjectInformation.Id);
			perfScope.AddAttachment("Project Path", rawProject.ProjectInformation.Path);

			await Task.Delay(1000);
		}

		stateSubject.OnNext(EInstanceState.Initialized);
	}

	private async Task TerminateAsync()
	{
		using var perfScope = dScope.CreatePerfMonitor(new PerfMonitorCreateOptions()
			{ Tags = new[] { nameof(ProjectInstance), "Terminate" } });

		perfScope.AddAttachment("Project Id", rawProject.ProjectInformation.Id);
		perfScope.AddAttachment("Project Path", rawProject.ProjectInformation.Path);

		await Task.Delay(1000);
	}

	public void Dispose()
	{
		if (stateSubject.Value == EInstanceState.Disposed)
			return;

		stateSubject.OnNext(EInstanceState.Disposed);

		TerminateAsync().ConfigureAwait(false).GetAwaiter().GetResult();

		scope.Dispose();
	}

	public IProjectInstance MakeRef()
	{
		// TODO
		return this;
	}
}