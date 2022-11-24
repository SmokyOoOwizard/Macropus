using System.Data;
using System.Reactive.Disposables;
using Macropus.Database;
using Macropus.DatabasesProvider.Db;
using Macropus.DatabasesProvider.Db.Models;
using Macropus.DatabasesProvider.Sqlite;
using Macropus.FileSystem.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Macropus.DatabasesProvider.Impl;

public class DatabasesProvider : IDatabasesProvider
{
	private readonly DatabasesDbContext context;
	private readonly IFileSystemProvider fileSystemProvider;

	private DatabasesProvider(DatabasesDbContext context, IFileSystemProvider fileSystemProvider)
	{
		this.context = context;
		this.fileSystemProvider = fileSystemProvider;
	}


	public async Task<IDbConnection?> TryGetDatabase(string name, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

		try
		{
			var db = await context.Databases.FirstOrDefaultAsync(d => d.Name == name, cancellationToken)
				.ConfigureAwait(false);
			if (db == null) return null;

			var dbFile = await fileSystemProvider.GetFileAsync(db.FileId, FileAccess.ReadWrite, FileShare.ReadWrite,
					cancellationToken)
				.ConfigureAwait(false) as IFileProviderInternal;

			if (dbFile == null)
				// TODO
				throw new Exception();

			return new SqliteConnectionWithIFile(dbFile);
		}
		catch
		{
			// TODO we need log it somewhere
			return null;
		}
	}

	public async Task<bool> CreateDatabaseAsync(string name, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

		if (await context.Databases.AnyAsync(d => d.Name == name, cancellationToken))
			return false;

		await using var trans = await context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
		try
		{
			var dbFileId = await fileSystemProvider.CreateFileAsync("Database." + name, cancellationToken)
				.ConfigureAwait(false);
			try
			{
				context.Databases.Add(new DatabaseDbModel { FileId = dbFileId, Name = name });

				await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

				await trans.CommitAsync().ConfigureAwait(false);
				return true;
			}
			catch
			{
				await fileSystemProvider.DeleteFileAsync(dbFileId).ConfigureAwait(false);
				throw;
			}
		}
		catch
		{
			await trans.RollbackAsync();
			// TODO we need log it somewhere
			return false;
		}
	}

	public async Task<bool> DeleteDatabaseAsync(string name, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

		try
		{
			var db = await context.Databases.FirstOrDefaultAsync(db => db.Name == name, cancellationToken)
				.ConfigureAwait(false);
			if (db == null)
				return false;

			context.Databases.Remove(db);

			await context.SaveChangesAsync().ConfigureAwait(false);

			await fileSystemProvider.DeleteFileAsync(db.FileId);

			return true;
		}
		catch
		{
			// TODO we need log it somewhere
			return false;
		}
	}

	public void Dispose()
	{
		context.Dispose();
		fileSystemProvider.Dispose();
	}

	public static async Task<DatabasesProvider> Create(
		string path,
		IFileSystemProvider fileSystemProvider,
		CancellationToken cancellationToken = default
	)
	{
		var disposable = new CompositeDisposable(Disposable.Empty);

		try
		{
			var dbContext = await GetOrCreateDbContextAsync(path, cancellationToken);
			disposable.Add(dbContext);

			return new DatabasesProvider(dbContext, fileSystemProvider);
		}
		catch
		{
			disposable.Dispose();
			throw;
		}
	}

	private static async Task<DatabasesDbContext> GetOrCreateDbContextAsync(
		string path,
		CancellationToken cancellationToken = default
	)
	{
		var dbProvider = new SqliteDbProvider($"Data Source={path}", path, Guid.Empty);

		await using (var dbConnection = dbProvider.CreateConnection())
		{
			await dbConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

			if (DbUtils.GetTableCount(dbConnection) == 0)
				await DbUtils.MigrateDb<DatabasesProviderDbMigrationsProvider>(dbConnection, 0,
						DatabasesProviderDbMigrationsProvider.LastVersion, cancellationToken)
					.ConfigureAwait(false);
		}


		return new DatabasesDbContext(dbProvider.CreateConnection());
	}
}