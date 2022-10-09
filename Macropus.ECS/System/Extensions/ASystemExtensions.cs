using System.Reflection;

namespace Macropus.ECS.System.Extensions;

public static class ASystemExtensions
{
	public static ComponentsFilter? GetFilter<T>(this T system) where T : ASystem
	{
		try
		{
			var type = system.GetType();
			if (!type.IsAssignableTo(typeof(IFilteredSystem)))
				return null;

			var method = type.GetMethod(nameof(IFilteredSystem.Filter));
			if (method == null)
				// TODO internal error
				throw new Exception();

			return method.Invoke(null, null) as ComponentsFilter;
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