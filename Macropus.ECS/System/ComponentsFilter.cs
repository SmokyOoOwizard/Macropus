using Macropus.ECS.Component;
using Macropus.ECS.ComponentsStorage;
using Macropus.ECS.System.Exceptions;

namespace Macropus.ECS.System;

// TODO
public class ComponentsFilter
{
	private ComponentsFilter() { }

	public IEnumerable<Guid> Filter(IReadOnlyComponentsStorage components)
	{
		return components.GetEntities();
	}

	public static ComponentsFilter AllOf(params ComponentsFilter[] filters)
	{
		return new ComponentsFilter();
	}

	public static ComponentsFilter AllOf(params Type[] components)
	{
		CheckComponents(components);
		return new ComponentsFilter();
	}

	public static ComponentsFilter AnyOf(params ComponentsFilter[] filters)
	{
		return new ComponentsFilter();
	}

	public static ComponentsFilter AnyOf(params Type[] components)
	{
		CheckComponents(components);
		return new ComponentsFilter();
	}

	public static ComponentsFilter NoneOf(params ComponentsFilter[] filters)
	{
		return new ComponentsFilter();
	}

	public static ComponentsFilter NoneOf(params Type[] components)
	{
		CheckComponents(components);
		return new ComponentsFilter();
	}

	private static void CheckComponents(Type[] components)
	{
		List<Type> nonComponents = new();
		foreach (var type in components)
		{
			if (!type.IsAssignableTo(typeof(IComponent)))
				nonComponents.Add(type);
		}

		if (nonComponents.Count > 0)
			throw new FilterHasTypesWhichAreNotComponentsException(nonComponents.ToArray());
	}
}