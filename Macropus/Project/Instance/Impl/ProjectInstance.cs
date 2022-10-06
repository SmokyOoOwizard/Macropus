using System.Reactive.Disposables;
using Macropus.Module;
using Macropus.Module.Impl;
using Macropus.Project.Provider;

namespace Macropus.Project.Instance.Impl;

internal sealed class ProjectInstance : IProjectInstance
{
    private readonly IProjectProvider projectProvider;

    private readonly ModulesInstance modulesInstance;

    private bool disposed;

    private ProjectInstance(IProjectProvider projectProvider, ModulesInstance modulesInstance)
    {
        this.projectProvider = projectProvider;
        this.modulesInstance = modulesInstance;
    }


    public void Dispose()
    {
        if (disposed) return;
        disposed = true;

        modulesInstance.Dispose();
        projectProvider.Dispose();
    }

    public static async Task<IProjectInstance> Create(IProjectProvider projectProvider,
        CancellationToken cancellationToken = default)
    {
        var disposable = new CompositeDisposable(Disposable.Empty);

        try
        {
            var modules = await ModulesInstance
                .Create(projectProvider.ModuleProvider, new ModulesCache(), cancellationToken)
                .ConfigureAwait(false);
            disposable.Add(modules);


            return new ProjectInstance(projectProvider, modules);
        }
        catch
        {
            disposable.Dispose();
            throw;
        }
    }
}