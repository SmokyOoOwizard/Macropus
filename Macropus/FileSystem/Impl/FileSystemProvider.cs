using System.Reactive.Disposables;
using Macropus.Database;
using Macropus.Database.Sqlite;
using Macropus.FileSystem.Db;
using Macropus.FileSystem.Db.Models.File;
using Macropus.Project;
using Macropus.Stuff;
using Microsoft.EntityFrameworkCore;

namespace Macropus.FileSystem.Impl;

public class FileSystemProvider : IFileSystemProvider
{
	private readonly string path;
	private readonly LockFile lockFile;
	private readonly FileSystemDbContext dbContext;

	private bool disposed;

	private FileSystemProvider(string path, LockFile lockFile, FileSystemDbContext dbContext)
	{
		this.path = path;
		this.lockFile = lockFile;
		this.dbContext = dbContext;
	}


	public async Task<IFileProvider> GetFileAsync(
		Guid id,
		FileAccess access,
		FileShare share,
		CancellationToken cancellationToken = default
	)
	{
		// TODO access and share not implemented

		if (Guid.Empty == id) throw new ArgumentNullException(nameof(id));

		var fileDb = await dbContext.Files.FirstOrDefaultAsync(f => f.Id == id, cancellationToken)
			.ConfigureAwait(false);
		if (fileDb == null)
			throw new FileNotFoundException();

		return FileProvider.Create(Path.Combine(path, fileDb.ObjectName), fileDb.Name, fileDb.Id, FileMode.OpenOrCreate,
			access, share);
	}

	public async Task<Guid> CreateFileAsync(string name, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

		var fileDb = new FileDbModel
		{
			Id = Guid.NewGuid(),
			Name = name
		};

		fileDb.ObjectName = GenerateFileName(fileDb.Id.ToString("n"));

		dbContext.Files.Add(fileDb);
		await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return fileDb.Id;
	}

	public async Task DeleteFileAsync(Guid id, CancellationToken cancellationToken = default)
	{
		if (Guid.Empty == id) throw new ArgumentNullException(nameof(id));

		var fileDb = await dbContext.Files.FirstOrDefaultAsync(f => f.Id == id, cancellationToken)
			.ConfigureAwait(false);
		if (fileDb == null)
			return;

		dbContext.Files.Remove(fileDb);

		await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		try
		{
			File.Delete(Path.Combine(path, fileDb.ObjectName));
		}
		catch (IOException) { }
	}

	private string GenerateFileName(string fileName)
	{
		var prefixName = "";
		for (var i = 0; i < 256; i++)
		{
			var tmpPath = Path.Combine(path, prefixName, fileName);
			if (!File.Exists(tmpPath)) return Path.Combine(prefixName, fileName);

			var p = i % (fileName.Length / 2);

			prefixName = Path.Combine(prefixName, fileName.Substring(p * 2, 2));
		}

		// TODO: throw internal error(can't find possibly file path)
		throw new Exception();
	}

	public void Dispose()
	{
		if (disposed) return;
		disposed = true;

		lockFile.Dispose();
	}

	public async ValueTask DisposeAsync()
	{
		if (disposed) return;
		disposed = true;

		await lockFile.DisposeAsync();
	}

	public static async Task<IFileSystemProvider> Create(string path, CancellationToken cancellationToken = default)
	{
		var disposable = new CompositeDisposable(Disposable.Empty);

		try
		{
			path = Path.Combine(path, ProjectPaths.FS_OBJECTS_PATH);

			if (!Directory.Exists(path)) Directory.CreateDirectory(path);

			var lockFile = await LockFile.LockWhileAsync(path, cancellationToken: cancellationToken);
			disposable.Add(lockFile);

			var dbContext = await GetOrCreateDbContextAsync(path, cancellationToken);
			disposable.Add(dbContext);

			return new FileSystemProvider(path, lockFile, dbContext);
		}
		catch
		{
			disposable.Dispose();
			throw;
		}
	}

	private static async Task<FileSystemDbContext> GetOrCreateDbContextAsync(
		string path,
		CancellationToken cancellationToken = default
	)
	{
		var dbPath = Path.Combine(path, ProjectPaths.FS_OBJECTS_DB_NAME);

		var dbProvider = new SqliteDbProvider($"Data Source={dbPath}", ProjectPaths.FS_OBJECTS_DB_NAME, Guid.Empty);

		using (var dbConnection = dbProvider.CreateConnection())
		{
			await dbConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

			if (DbUtils.GetTableCount(dbConnection) == 0)
				await DbUtils.MigrateDb<FileSystemDbMigrationsProvider>(dbConnection, 0,
						FileSystemDbMigrationsProvider.LastVersion, cancellationToken)
					.ConfigureAwait(false);
		}


		var dbContext = new FileSystemDbContext(dbProvider.CreateConnection());

		return dbContext;
	}
}