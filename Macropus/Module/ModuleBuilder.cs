using Autofac;
using Macropus.Interfaces.Module;

namespace Macropus.Module;

internal class ModuleBuilder : Autofac.Module, IModuleBuilder
{
	public static ContainerBuilder Merge(IEnumerable<ModuleBuilder> builders)
	{
		var mergeBuilder = new ContainerBuilder();

		foreach (var moduleBuilder in builders) mergeBuilder.RegisterModule(moduleBuilder);

		return mergeBuilder;
	}
}