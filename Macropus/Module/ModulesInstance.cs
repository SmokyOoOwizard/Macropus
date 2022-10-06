using System.Reactive.Disposables;
using Macropus.Module.Extensions;

namespace Macropus.Module;

internal sealed class ModulesInstance : IDisposable
{
    private readonly List<IModuleContainer> modules;

    private bool disposed;

    private ModulesInstance(List<IModuleContainer> containers)
    {
        modules = containers;
    }

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;

        for (var i = 0; i < modules.Count; i++) modules[i].Dispose();

        modules.Clear();
    }

    public static async Task<ModulesInstance> Create(IModulesProvider modulesProvider, IModulesCache modulesCache,
        CancellationToken cancellationToken = default)
    {
        var disposable = new CompositeDisposable(Disposable.Empty);

        try
        {
            var modules = await modulesProvider.GetModulesInfoAsync(cancellationToken).ConfigureAwait(false);
            modules = modules.Where(m => m.Enable).ToArray();

            var loadedModules = new List<IModuleContainer>();
            foreach (var moduleInfo in modules)
            {
                using var fileProvider = await modulesProvider.GetModuleAsync(moduleInfo, cancellationToken)
                    .ConfigureAwait(false);

                var loadedModuleRef = await modulesCache.GetOrLoadModuleAsync(fileProvider, cancellationToken)
                    .ConfigureAwait(false);

                var module = loadedModuleRef.ToSharedModule();
                disposable.Add(module);
                loadedModules.Add(module);
            }

            // TODO....


            return new ModulesInstance(loadedModules);
        }
        catch
        {
            disposable.Dispose();
            throw;
        }
    }
}