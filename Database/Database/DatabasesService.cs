using System.Data;
using Delogger.Scope;
using Delogger.Scope.Log;
using Macropus.Database.Db;
using Macropus.Database.Db.Models;
using Macropus.Database.Interfaces;
using Macropus.Database.Sqlite;
using Macropus.FileSystem.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Macropus.Database;

internal class DatabasesService : IDatabasesService
{
	private readonly DatabasesDbContext context;
	private readonly IFileSystemService fileSystemService;
	private readonly IDScope scope;
	private readonly IDLogger logger;

	public DatabasesService(DatabasesDbContext context, IFileSystemService fileSystemService, IDScope scope)
	{
		this.context = context;
		this.fileSystemService = fileSystemService;
		this.scope = scope;
		logger = scope.CreateLogger(new LoggerCreateOptions() { Tags = new[] { nameof(DatabasesService) } });
	}


	public async Task<IDbConnection?> TryGetDatabase(string name, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentNullException(nameof(name));

		try
		{
			var db = await context.Databases.FirstOrDefaultAsync(d => d.Name == name, cancellationToken)
				.ConfigureAwait(false);
			if (db == null) return null;

			var dbFile = await fileSystemService.GetFileAsync(db.FileId, FileAccess.ReadWrite, FileShare.ReadWrite,
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
			var dbFileId = await fileSystemService.CreateFileAsync("Database." + name, cancellationToken)
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
				await fileSystemService.DeleteFileAsync(dbFileId).ConfigureAwait(false);
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

			await fileSystemService.DeleteFileAsync(db.FileId);

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
		fileSystemService.Dispose();
	}
}