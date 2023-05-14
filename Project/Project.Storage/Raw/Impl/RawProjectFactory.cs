using System.Reactive.Disposables;
using Autofac;
using Macropus.CoolStuff;
using Macropus.Database.Interfaces;
using Macropus.Extensions;
using Macropus.FileSystem.Interfaces;
using Macropus.Project.Raw.Impl;

namespace Macropus.Project.Storage.Raw.Impl;

public class RawProjectFactory
{
	private readonly ILifetimeScope scope;
	private readonly IFileSystemServiceFactory fileSystemServiceFactory;
	private readonly IDatabasesServiceFactory databasesServiceFactory;

	public RawProjectFactory(
		ILifetimeScope scope,
		IFileSystemServiceFactory fileSystemServiceFactory,
		IDatabasesServiceFactory databasesServiceFactory
	)
	{
		this.scope = scope;
		this.fileSystemServiceFactory = fileSystemServiceFactory;
		this.databasesServiceFactory = databasesServiceFactory;
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

			var fileSystemService = await fileSystemServiceFactory.Create(
					ProjectPaths.FS_OBJECTS_PATH, ProjectPaths.FS_OBJECTS_DB_NAME, cancellationToken)
				.ConfigureAwait(false);
			fileSystemService.AddTo(disposable);

			var databasesService = await databasesServiceFactory
				.Create(ProjectPaths.DATABASES_PROVIDER_DB_NAME, fileSystemService, cancellationToken)
				.ConfigureAwait(false);
			databasesService.AddTo(disposable);

			var instanceScope = scope.BeginLifetimeScope(cb =>
			{
				cb.RegisterInstance(projectInfo);
				cb.RegisterInstance(fileSystemService)
					.AsSelf()
					.AsImplementedInterfaces()
					.SingleInstance();
				cb.RegisterInstance(databasesService)
					.AsSelf()
					.AsImplementedInterfaces()
					.SingleInstance();
			});
			instanceScope.AddTo(disposable);

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