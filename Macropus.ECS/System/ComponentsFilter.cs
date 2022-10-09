using Macropus.ECS.Component;
using Macropus.ECS.ComponentsStorage;
using Macropus.ECS.System.Exceptions;

namespace Macropus.ECS.System;

public readonly struct ComponentsFilter
{
	private readonly EComponentsFilterType type;

	private readonly ComponentsFilter[] subFilters;
	private readonly string[] filterComponents;

	private ComponentsFilter(EComponentsFilterType type, ComponentsFilter[] subFilters, string[] filterComponents)
	{
		this.type = type;
		this.subFilters = subFilters;
		this.filterComponents = filterComponents;
	}

	public IEnumerable<Guid> Filter(IReadOnlyComponentsStorage components)
	{
		foreach (var entity in components.GetEntities())
		{
			if (EntityFitsFilter(entity, components))
				yield return entity;
		}
	}

	private bool EntityFitsFilter(Guid entity, IReadOnlyComponentsStorage components)
	{
		switch (type)
		{
			case EComponentsFilterType.All:
			{
				foreach (var component in filterComponents)
				{
					if (!components.HasComponent(entity, component))
						return false;
				}

				foreach (var filter in subFilters)
				{
					if (!filter.EntityFitsFilter(entity, components))
						return false;
				}

				return true;
			}
			case EComponentsFilterType.Any:
			{
				foreach (var component in filterComponents)
				{
					if (components.HasComponent(entity, component))
						return true;
				}

				foreach (var filter in subFilters)
				{
					if (filter.EntityFitsFilter(entity, components))
						return true;
				}

				return false;
			}
			case EComponentsFilterType.None:
			{
				foreach (var component in filterComponents)
				{
					if (components.HasComponent(entity, component))
						return false;
				}

				foreach (var filter in subFilters)
				{
					if (filter.EntityFitsFilter(entity, components))
						return false;
				}

				return true;
			}
		}

		return false;
	}

	public static ComponentsFilter AllOf(params ComponentsFilter[] filters)
	{
		return new ComponentsFilter(
			EComponentsFilterType.All,
			filters,
			Array.Empty<string>()
		);
	}

	public static ComponentsFilter AllOf(params Type[] components)
	{
		CheckComponents(components);
		return new ComponentsFilter(
			EComponentsFilterType.All,
			Array.Empty<ComponentsFilter>(),
			components.Select(t => t.FullName).ToArray()!
		);
	}

	public static ComponentsFilter AnyOf(params ComponentsFilter[] filters)
	{
		return new ComponentsFilter(
			EComponentsFilterType.Any,
			filters,
			Array.Empty<string>()
		);
	}

	public static ComponentsFilter AnyOf(params Type[] components)
	{
		CheckComponents(components);
		return new ComponentsFilter(
			EComponentsFilterType.Any,
			Array.Empty<ComponentsFilter>(),
			components.Select(t => t.FullName).ToArray()!
		);
	}

	public static ComponentsFilter NoneOf(params ComponentsFilter[] filters)
	{
		return new ComponentsFilter(
			EComponentsFilterType.None,
			filters,
			Array.Empty<string>()
		);
	}

	public static ComponentsFilter NoneOf(params Type[] components)
	{
		CheckComponents(components);
		return new ComponentsFilter(
			EComponentsFilterType.None,
			Array.Empty<ComponentsFilter>(),
			components.Select(t => t.FullName).ToArray()!
		);
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