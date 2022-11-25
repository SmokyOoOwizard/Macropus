using System.Data.Common;
using System.Reactive.Disposables;
using Autofac;
using Delogger;
using Delogger.Scope;
using Delogger.Scope.Log;
using Macropus.Database.Interfaces;
using Macropus.FileSystem.Db;
using Macropus.FileSystem.Interfaces;

namespace Macropus.FileSystem;

public class FileSystemProviderFactory
{
	private readonly ILifetimeScope scope;
	private readonly IDbContextService dbContextService;
	private readonly IDScope dScope;
	private readonly IDLogger logger;

	public FileSystemProviderFactory(ILifetimeScope scope, IDbContextService dbContextService, IDScope dScope)
	{
		this.scope = scope;
		this.dbContextService = dbContextService;
		this.dScope = dScope;
		logger = dScope.CreateLogger(new LoggerCreateOptions() { Tags = new[] { nameof(FileSystemProviderFactory) } });
	}

	public async Task<IFileSystemService> Create(
		string objLocation,
		DbConnection dbConnection,
		CancellationToken cancellationToken = default
	)
	{
		var disposable = new CompositeDisposable(Disposable.Empty);

		try
		{
			if (!Directory.Exists(objLocation))
				Directory.CreateDirectory(objLocation);

			var dbContext =
				await dbContextService.GetOrCreateDbContextAsync<FileSystemDbContext, FileSystemDbMigrationsProvider>(
					dbConnection, cancellationToken);
			disposable.Add(dbContext);
			
			return scope.Resolve<FileSystemService>(
				new PositionalParameter(0, scope.BeginLifetimeScope()),
				new PositionalParameter(1, objLocation),
				new PositionalParameter(2, dbContext)
			);
		}
		catch (Exception ex)
		{
			disposable.Dispose();
			logger.LogException(ex);
			throw;
		}
	}

	public async Task<IFileSystemService> Create(
		string objLocation,
		string dbPath,
		CancellationToken cancellationToken = default
	)
	{
		var disposable = new CompositeDisposable(Disposable.Empty);

		try
		{
			if (!Directory.Exists(objLocation))
				Directory.CreateDirectory(objLocation);

			var dbContext =
				await dbContextService.GetOrCreateDbContextAsync<FileSystemDbContext, FileSystemDbMigrationsProvider>(
					dbPath, cancellationToken);
			disposable.Add(dbContext);

			return scope.Resolve<FileSystemService>(
				new PositionalParameter(0, scope.BeginLifetimeScope()),
				new PositionalParameter(1, objLocation),
				new PositionalParameter(2, dbContext)
			);
		}
		catch (Exception ex)
		{
			disposable.Dispose();
			logger.LogException(ex);
			throw;
		}
	}
}