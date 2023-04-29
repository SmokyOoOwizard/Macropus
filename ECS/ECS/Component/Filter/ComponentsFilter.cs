using Macropus.ECS.Component.Storage;

namespace Macropus.ECS.Component.Filter;

public readonly struct ComponentsFilter
{
	public static ComponentsFilter Empty => new((_, _) => true);

	private readonly Func<Guid, IReadOnlyComponentsStorage, bool> filter;

	internal ComponentsFilter(Func<Guid, IReadOnlyComponentsStorage, bool> filter)
	{
		this.filter = filter;
	}


	public bool Filter(Guid entityId, IReadOnlyComponentsStorage storage)
	{
		return filter.Invoke(entityId, storage);
	}

	public static ComponentsFilterBuilder AllOf(params ComponentsFilterBuilder[] filters)
		=> new(EComponentsFilterType.All, filters);

	public static ComponentsFilterBuilder AllOf(params Type[] components)
		=> new(EComponentsFilterType.All, components);

	public static ComponentsFilterBuilder AnyOf(params ComponentsFilterBuilder[] filters)
		=> new(EComponentsFilterType.Any, filters);

	public static ComponentsFilterBuilder AnyOf(params Type[] components)
		=> new(EComponentsFilterType.Any, components);

	public static ComponentsFilterBuilder NoneOf(params ComponentsFilterBuilder[] filters)
		=> new(EComponentsFilterType.None, filters);

	public static ComponentsFilterBuilder NoneOf(params Type[] components)
		=> new(EComponentsFilterType.None, components);
}