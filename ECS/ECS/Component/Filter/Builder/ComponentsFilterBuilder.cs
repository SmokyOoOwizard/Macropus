using Macropus.ECS.Systems.Exceptions;

namespace Macropus.ECS.Component.Filter.Builder;

public readonly struct ComponentsFilterBuilder
{
	internal readonly EComponentsFilterType Type;

	internal readonly ComponentsFilterBuilder[] SubFilters;
	internal readonly Type[] FilterComponents;

	public ComponentsFilterBuilder(
		EComponentsFilterType type,
		params Type[] filterComponents
	)
	{
		if (filterComponents.Length == 0)
			throw new ArgumentOutOfRangeException();

		CheckComponents(filterComponents);

		Type = type;
		SubFilters = Array.Empty<ComponentsFilterBuilder>();
		FilterComponents = filterComponents;
	}

	public ComponentsFilterBuilder(
		EComponentsFilterType type,
		params ComponentsFilterBuilder[] subFilters
	)
	{
		if (subFilters.Length == 0)
			throw new ArgumentOutOfRangeException();

		Type = type;
		SubFilters = subFilters;
		FilterComponents = Array.Empty<Type>();
	}

	public ComponentsFilter Build()
	{
		return new ComponentsFilter(
			ComponentsStorageFilterBuilder.Build(this).Compile(),
			ComponentCollectionFilterBuilder.Build(this).Compile()
		);
	}


	private static void CheckComponents(IEnumerable<Type> components)
	{
		var nonComponents = components.Where(type => !type.IsAssignableTo(typeof(IComponent))).ToList();
		if (nonComponents.Count > 0)
			throw new TypesAreNotComponentsException(nonComponents.ToArray());
	}
}