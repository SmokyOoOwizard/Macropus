namespace Macropus.ECS.System.Extensions;

public static class ASystemExtensions
{
	public static ISystemFilter? GetFilter<T>(this T _) where T : ASystem
	{
		var type = typeof(T);
		if (!type.IsAssignableTo(typeof(IFilteredSystem)))
			return null;

		var method = type.GetMethod(nameof(IFilteredSystem.Filter));
		if (method == null)
			// TODO internal error
			throw new Exception();

		return method.Invoke(null, null) as ISystemFilter;
	}
}