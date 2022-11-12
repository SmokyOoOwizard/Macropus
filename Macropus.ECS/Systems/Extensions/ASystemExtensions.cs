using System.Reflection;
using Macropus.ECS.Systems.Filter;

namespace Macropus.ECS.Systems.Extensions;

public static class ASystemExtensions
{
	public static ComponentsFilter? GetFilter<T>(this T system) where T : ASystem
	{
		try
		{
			var type = system.GetType();
			if (!type.IsAssignableTo(typeof(IReactiveSystem)))
				return null;

			var method = type.GetMethod(nameof(IReactiveSystem.Filter));
			if (method == null)
				// TODO internal error
				throw new Exception();

			return (ComponentsFilter)method.Invoke(null, null)!;
		}
		catch (Exception e)
		{
			if (e is TargetInvocationException tie)
				if (tie.InnerException != null)
					throw tie.InnerException;

			throw;
		}
	}
}