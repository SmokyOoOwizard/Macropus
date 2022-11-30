using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Autofac;
using Macropus.Extensions;
using Macropus.Interfaces.Project;
using Macropus.Project.Instance;
using Macropus.Project.Instance.Impl;

namespace Macropus.Project.Connection.Impl;

internal class ProjectConnection : IProjectConnection
{
	private readonly IProjectInstance project;
	private readonly ILifetimeScope scope;

	private readonly BehaviorSubject<EConnectionState> stateSubject;
	private readonly CompositeDisposable disposable;
	public IObservable<EConnectionState> State => stateSubject.AsObservable();

	public IProjectInformation ProjectInformation => project.ProjectInformation;

	public ProjectConnection(IProjectInstance project, ILifetimeScope scope)
	{
		this.project = project;
		this.scope = scope;

		stateSubject = new(EConnectionState.Connection);

		disposable = new CompositeDisposable(Disposable.Empty);

		project.State
			.Sample(project.State)
			.Subscribe(OnProjectChangeState)
			.AddTo(disposable);
	}

	private void OnProjectChangeState(EInstanceState state)
	{
		switch (state)
		{
			case EInstanceState.NotInit:
				stateSubject.OnNext(EConnectionState.Connection);
				break;
			case EInstanceState.Initialization:
				stateSubject.OnNext(EConnectionState.WaitProject);
				break;
			case EInstanceState.Initialized:
				stateSubject.OnNext(EConnectionState.Connected);
				break;
			case EInstanceState.UNKNOWN:
			case EInstanceState.Disposed:
				stateSubject.OnNext(EConnectionState.UNKNOWN);
				break;
		}
	}

	public void Dispose()
	{
		stateSubject.OnNext(EConnectionState.Closed);
		disposable.Dispose();
		project.Dispose();
	}
}