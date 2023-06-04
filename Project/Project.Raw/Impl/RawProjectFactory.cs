using System.Reactive.Disposables;
using Autofac;
using Macropus.CoolStuff;
using Macropus.Database.Interfaces;
using Macropus.Extensions;
using Macropus.FileSystem.Interfaces;
using Macropus.Project.Impl;
using Macropus.Project.Raw.Raw.Impl;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Macropus.Project.Raw.Impl;

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

	public static async Task<IProjectInformationInternal?> TryGetProjectInfo(
		string path,
		CancellationToken cancellationToken = default
	)
	{
		if (!Directory.Exists(path))
			return null;

		if (!File.Exists(Path.Combine(path, ProjectPaths.PROJECT_FILE_NAME)))
			return null;

		var deserializer = new DeserializerBuilder()
			.WithNamingConvention(UnderscoredNamingConvention.Instance)
			.Build();

		await using var fs = new FileStream(Path.Combine(path, ProjectPaths.PROJECT_FILE_NAME), FileMode.Open);
		using var sr = new StreamReader(fs);

		var projInfo =
			deserializer.Deserialize<ProjectInformationInternal>(await sr.ReadToEndAsync(cancellationToken)
				.WithCancellation(cancellationToken));

		projInfo.Path = path;

		return projInfo;
	}

	public async Task<Guid> CreateProjectAsync(
		string path,
		IProjectCreationInfo creationInfo,
		CancellationToken cancellationToken = default
	)
	{
		var newPath = Path.Combine(path, creationInfo.Name);

		await RawProjectUtils.CreateProject(newPath, creationInfo, cancellationToken);

		var projectInfo = await TryGetProjectInfo(newPath, cancellationToken);

		if (projectInfo == null)
			throw new(); // TODO throw failed read project info

		return projectInfo.Id;
	}

	public async Task<IRawProject> Open(string path, CancellationToken cancellationToken = default)
	{
		if (!Directory.Exists(path))
			// TODO throw directory not exists
			throw new Exception();

		var projectInfo = await TryGetProjectInfo(path, cancellationToken).ConfigureAwait(false);
		if (projectInfo == null)
			throw new(); // TODO throw it's not project directory

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