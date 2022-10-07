using System.Reactive.Disposables;
using Autofac;
using Macropus.Module.Extensions;

namespace Macropus.Module;

internal sealed class ModulesInstance : IDisposable
{
	private readonly List<LoadedModule> modules;
	private readonly ContainerBuilder modulesBuilder;
	private readonly IContainer modulesContainer;

	private bool disposed;

	private ModulesInstance(List<LoadedModule> modules, ContainerBuilder modulesBuilder)
	{
		this.modules = modules;
		this.modulesBuilder = modulesBuilder;

		modulesContainer = modulesBuilder.Build();
	}

	public void Dispose()
	{
		if (disposed) return;
		disposed = true;

		modulesContainer.Dispose();
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

			var loadedModules = new List<LoadedModule>();
			foreach (var moduleInfo in modules)
			{
				var module = new LoadedModule();
				disposable.Add(module);
				loadedModules.Add(module);

				var rawModule = await LoadRawModule(modulesProvider, modulesCache, moduleInfo, cancellationToken)
					.ConfigureAwait(false);
				module.RawModuleContainer = rawModule;

				var moduleEntryPoint = rawModule.CreateEntryPoint();
				module.Module = moduleEntryPoint;

				// TODO get allowed permissions for module
				// TODO remove unrequested permissions

				await moduleEntryPoint.Initialize(null, cancellationToken).ConfigureAwait(false);

				var moduleBuilder = new ModuleBuilder();
				moduleEntryPoint.BindModule(moduleBuilder);
				module.Bindings = moduleBuilder;
			}

			var mergeBuilder = ModuleBuilder.Merge(loadedModules.Select(m => m.Bindings));

			return new ModulesInstance(loadedModules, mergeBuilder);
		}
		catch
		{
			disposable.Dispose();
			throw;
		}
	}

	private static async Task<IRawModuleContainer> LoadRawModule(
		IModulesProvider modulesProvider,
		IModulesCache modulesCache,
		IModuleInfo moduleInfo,
		CancellationToken cancellationToken)
	{
		using var fileProvider = await modulesProvider.GetModuleAsync(moduleInfo, cancellationToken)
			.ConfigureAwait(false);

		var loadedModuleRef = await modulesCache.GetOrLoadModuleAsync(fileProvider, cancellationToken)
			.ConfigureAwait(false);

		return loadedModuleRef.ToSharedModule();
	}
}