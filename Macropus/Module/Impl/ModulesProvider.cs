using System.Data.Common;
using System.Reactive.Disposables;
using Macropus.Database;
using Macropus.FileSystem.Interfaces;
using Macropus.Module.Db;

namespace Macropus.Module.Impl;

internal class ModulesProvider : IModulesProvider
{
	private readonly ModulesDbContext dbContext;
	private readonly IFileSystemProvider fsProvider;

	private bool disposed;

	public ModulesProvider(ModulesDbContext dbContext, IFileSystemProvider fsProvider)
	{
		this.dbContext = dbContext;
		this.fsProvider = fsProvider;
	}

	public Task<IModuleInfo[]> GetModulesInfoAsync(CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<IFileProvider> GetModuleAsync(IModuleInfo moduleInfo, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public void Dispose()
	{
		if (disposed) return;
		disposed = true;

		dbContext.Dispose();
	}

	public static async Task<ModulesProvider> Create(
		IFileSystemProvider fsProvider,
		DbConnection dbConnection,
		CancellationToken cancellationToken = default
	)
	{
		var disposable = new CompositeDisposable(Disposable.Empty);

		try
		{
			var dbContext = await GetOrCreateDbContextAsync(dbConnection, cancellationToken).ConfigureAwait(false);
			disposable.Add(dbContext);

			return new ModulesProvider(dbContext, fsProvider);
		}
		catch
		{
			disposable.Dispose();
			throw;
		}
	}

	private static async Task<ModulesDbContext> GetOrCreateDbContextAsync(
		DbConnection dbConnection,
		CancellationToken cancellationToken = default
	)
	{
		await dbConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

		if (DbUtils.GetTableCount(dbConnection) == 0)
		{
			// TODO
			//await DbUtils.MigrateDb<FileSystemDbMigrationsProvider>(dbConnection, 0, FileSystemDbMigrationsProvider.LastVersion, cancellationToken).ConfigureAwait(false);
		}

		var dbContext = new ModulesDbContext(dbConnection);

		return dbContext;
	}
}