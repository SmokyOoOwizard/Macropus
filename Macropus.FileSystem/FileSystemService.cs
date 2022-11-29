using Autofac;
using Delogger.Scope;
using Delogger.Scope.Log;
using Macropus.FileSystem.Db;
using Macropus.FileSystem.Db.Models.File;
using Macropus.FileSystem.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Macropus.FileSystem;

internal class FileSystemService : IFileSystemService
{
	private readonly ILifetimeScope scope;
	private readonly string path;
	private readonly FileSystemDbContext dbContext;
	private readonly IDScope dScope;
	private readonly IDLogger logger;

	private bool disposed;

	public FileSystemService(ILifetimeScope scope, string path, FileSystemDbContext dbContext)
	{
		this.scope = scope;
		this.path = path;
		this.dbContext = dbContext;
		dScope = scope.Resolve<IDScope>();
		logger = dScope.CreateLogger(new LoggerCreateOptions() { Tags = new[] { nameof(FileSystemService) } });
	}


	public async Task<IFileProvider> GetFileAsync(
		Guid id,
		FileAccess access,
		FileShare share,
		CancellationToken cancellationToken = default
	)
	{
		try
		{
			// TODO access and share not implemented

			if (Guid.Empty == id)
				throw new ArgumentNullException(nameof(id));

			var fileDb = await dbContext.Files.FirstOrDefaultAsync(f => f.Id == id, cancellationToken)
				.ConfigureAwait(false);
			if (fileDb == null)
				throw new FileNotFoundException();

			return scope.Resolve<FileProvider>(
				new PositionalParameter(0, Path.Combine(path, fileDb.ObjectName)),
				new PositionalParameter(1, fileDb.Name!),
				new PositionalParameter(2, fileDb.Id),
				new PositionalParameter(3, FileMode.OpenOrCreate),
				new PositionalParameter(4, access),
				new PositionalParameter(5, share));
		}
		catch (Exception e)
		{
			logger.Log(e.ToString(), new[]
			{
				"Error",
				nameof(GetFileAsync)
			}, null, new KeyValuePair<string, object>[]
			{
				new("File Id", id)
			});
			throw;
		}
	}

	public async Task<Guid> CreateFileAsync(string name, CancellationToken cancellationToken = default)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

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
		catch (Exception e)
		{
			logger.Log(e.ToString(), new[]
			{
				"Error",
				nameof(CreateFileAsync)
			}, null, new KeyValuePair<string, object>[]
			{
				new("File Name", name)
			});
			throw;
		}
	}

	public async Task DeleteFileAsync(Guid id, CancellationToken cancellationToken = default)
	{
		try
		{
			if (Guid.Empty == id)
				throw new ArgumentNullException(nameof(id));

			var fileDb = await dbContext.Files.FirstOrDefaultAsync(f => f.Id == id, cancellationToken)
				.ConfigureAwait(false);
			if (fileDb == null)
				return;

			dbContext.Files.Remove(fileDb);

			await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

			File.Delete(Path.Combine(path, fileDb.ObjectName));
		}
		catch (IOException) { }
		catch (Exception e)
		{
			logger.Log(e.ToString(), new[]
			{
				"Error",
				nameof(DeleteFileAsync)
			}, null, new KeyValuePair<string, object>[]
			{
				new("File Id", id)
			});
			throw;
		}
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

		scope.Dispose();
	}

	public async ValueTask DisposeAsync()
	{
		if (disposed)
			return;
		disposed = true;

		await scope.DisposeAsync();
	}
}