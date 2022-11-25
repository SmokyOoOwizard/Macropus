using System.Reactive.Disposables;
using Autofac;
using Delogger;
using Delogger.Scope;
using Delogger.Scope.Log;
using Macropus.Database.Db;
using Macropus.Database.Interfaces;
using Macropus.FileSystem.Interfaces;

namespace Macropus.Database;

internal class DatabasesServiceFactory : IDatabasesServiceFactory
{
	private readonly ILifetimeScope scope;
	private readonly IDbContextService dbContextService;
	private readonly IDScope dScope;
	private readonly IDLogger logger;

	public DatabasesServiceFactory(ILifetimeScope scope, IDbContextService dbContextService, IDScope dScope)
	{
		this.scope = scope;
		this.dbContextService = dbContextService;
		this.dScope = dScope;
		logger = dScope.CreateLogger(new LoggerCreateOptions() { Tags = new[] { nameof(DatabasesServiceFactory) } });
	}

	public async Task<IDatabasesService> Create(
		string path,
		IFileSystemProvider fileSystemProvider,
		CancellationToken cancellationToken = default
	)
	{
		var disposable = new CompositeDisposable(Disposable.Empty);

		try
		{
			var dbContext =
				await dbContextService
					.GetOrCreateDbContextAsync<DatabasesDbContext, DatabasesProviderDbMigrationsProvider>(path, cancellationToken);
			disposable.Add(dbContext);

			return new DatabasesService(dbContext, fileSystemProvider, scope.Resolve<IDScope>());
		}
		catch (Exception ex)
		{
			disposable.Dispose();
			logger.LogException(ex);
			throw;
		}
	}
}