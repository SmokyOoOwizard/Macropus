using System.Reactive.Disposables;
using Autofac;
using Macropus.CoolStuff;

namespace Macropus.Project.Provider.Impl;

public class ProjectProviderFactory
{
	private readonly ILifetimeScope scope;

	public ProjectProviderFactory(ILifetimeScope scope)
	{
		this.scope = scope;
	}

	public async Task<IProjectProvider> Create(
		string path,
		CancellationToken cancellationToken = default
	)
	{
		if (!Directory.Exists(path))
			// TODO throw directory not exists
			throw new Exception();

		var projectInfo = await ProjectUtils.TryGetProjectInfo(path, cancellationToken).ConfigureAwait(false);
		if (projectInfo == null)
			// TODO throw it's not project directory
			throw new Exception();

		var disposable = new CompositeDisposable(Disposable.Empty);

		try
		{
			var lockFile = await LockFile.LockWhileAsync(path, cancellationToken: cancellationToken)
				.ConfigureAwait(false);
			disposable.Add(lockFile);

			var instanceScope = scope.BeginLifetimeScope(cb => cb.RegisterInstance(projectInfo));
			disposable.Add(instanceScope);

			return new ProjectProvider(lockFile, instanceScope);
		}
		catch
		{
			disposable.Dispose();
			throw;
		}
	}
}