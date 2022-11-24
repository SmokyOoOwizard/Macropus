using System.Reactive.Disposables;
using Autofac;
using Macropus.CoolStuff;

namespace Macropus.Project.Raw.Impl;

public class RawProjectFactory
{
	private readonly ILifetimeScope scope;

	public RawProjectFactory(ILifetimeScope scope)
	{
		this.scope = scope;
	}
	
	public async Task<IRawProject> Create(string path, CancellationToken cancellationToken = default)
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

			var provider = new RawProject(lockFile, instanceScope);

			return provider;
		}
		catch
		{
			disposable.Dispose();
			throw;
		}
	}
}