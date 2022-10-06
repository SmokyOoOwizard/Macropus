using System.Reactive.Disposables;
using Macropus.Db;
using Macropus.FileSystem;
using Macropus.Module.Db;
using Macropus.Project;
using Macropus.Stuff;
using Macropus.Utils;

namespace Macropus.Module.Impl;

internal class ModulesProvider : IModulesProvider
{
    private readonly LockFile lockFile;
    private readonly ModulesDbContext dbContext;
    private readonly IFileSystemProvider fsProvider;

    private bool disposed;

    public ModulesProvider(LockFile lockFile, ModulesDbContext dbContext, IFileSystemProvider fsProvider)
    {
        this.lockFile = lockFile;
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
        lockFile.Dispose();
    }

    public static async Task<ModulesProvider> Create(string path, IFileSystemProvider fsProvider,
        CancellationToken cancellationToken = default)
    {
        var disposable = new CompositeDisposable(Disposable.Empty);

        try
        {
            if (!Directory.Exists(path)) throw new DirectoryNotFoundException();

            var lockFile = await LockFile.LockWhileAsync(path, "modules.lock", cancellationToken)
                .ConfigureAwait(false);
            disposable.Add(lockFile);

            var dbContext = await GetOrCreateDbContextAsync(path, cancellationToken).ConfigureAwait(false);
            disposable.Add(dbContext);

            return new ModulesProvider(lockFile, dbContext, fsProvider);
        }
        catch
        {
            disposable.Dispose();
            throw;
        }
    }

    private static async Task<ModulesDbContext> GetOrCreateDbContextAsync(string path,
        CancellationToken cancellationToken = default)
    {
        var dbPath = Path.Combine(path, ProjectPaths.MODULES_DB_NAME);

        var dbProvider = new SqliteDbProvider($"Data Source={dbPath}", ProjectPaths.MODULES_DB_NAME, Guid.Empty);

        using (var dbConnection = dbProvider.CreateConnection())
        {
            await dbConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

            if (DbUtils.GetTableCount(dbConnection) == 0)
            {
                // TODO
                //await DbUtils.MigrateDb<FileSystemDbMigrationsProvider>(dbConnection, 0, FileSystemDbMigrationsProvider.LastVersion, cancellationToken).ConfigureAwait(false);
            }
        }


        var dbContext = new ModulesDbContext(dbProvider.CreateConnection());

        return dbContext;
    }
}