using Macropus.Interfaces.Module;

namespace Macropus.Extensions;

public static class ModuleExtensions
{
	public static IModuleRequiresPermissions? GetRequiresPermissions<T>(this T _) where T : IModuleBase
	{
		var type = typeof(T);
		if (!type.IsAssignableTo(typeof(IModule)))
			// TODO module don't implement IModule interface
			throw new Exception();

		var method = type.GetMethod(nameof(IModule.GetRequiresPermissions));
		if (method == null)
			// TODO internal error
			throw new Exception();

		return method.Invoke(null, null) as IModuleRequiresPermissions;
	}
}