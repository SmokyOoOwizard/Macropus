using System.Reactive.Disposables;
using Autofac;
using Macropus.FileSystem;
using Macropus.Module;
using Macropus.Project.Instance;
using Macropus.Stuff;

namespace Macropus.Project.Provider.Impl;

internal class ProjectProvider : IProjectProvider
{
    private readonly LockFile lockFile;
    private readonly ILifetimeScope container;

    public IProjectInformationInternal ProjectInformation { get; }
    public IFileSystemProvider FilesProvider { get; }
    public IModulesProvider ModuleProvider { get; }

    private bool disposed;

    public ProjectProvider(LockFile lockFile, ILifetimeScope container)
    {
        this.lockFile = lockFile;
        this.container = container;

        ProjectInformation = container.Resolve<IProjectInformationInternal>();
        FilesProvider = container.Resolve<IFileSystemProvider>();
        ModuleProvider = container.Resolve<IModulesProvider>();
    }

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;

        container.Dispose();

        lockFile.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (disposed) return;
        disposed = true;

        await container.DisposeAsync();

        await lockFile.DisposeAsync();
    }

    ~ProjectProvider()
    {
        Dispose();
    }

    public static async Task<IProjectProvider> Create(string path, CancellationToken cancellationToken = default)
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

            var projectContainer = ProjectProviderContainerBuilder.GetContainer();
            var instanceScope = projectContainer.BeginLifetimeScope(cb => cb.RegisterInstance(projectInfo));
            disposable.Add(instanceScope);

            return new ProjectProvider(lockFile, instanceScope);
        }
        catch
        {
            disposable.Dispose();
            throw;
        }
    }
}