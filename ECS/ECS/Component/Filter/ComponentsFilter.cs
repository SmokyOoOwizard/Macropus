using Macropus.ECS.Component.Storage;

namespace Macropus.ECS.Component.Filter;

public readonly struct ComponentsFilter
{
	public static ComponentsFilter Empty => new();

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
		=> ComponentsFilterBuilder.AllOf(filters);

	public static ComponentsFilterBuilder AllOf(params Type[] components)
		=> ComponentsFilterBuilder.AllOf(components);

	public static ComponentsFilterBuilder AnyOf(params ComponentsFilterBuilder[] filters)
		=> ComponentsFilterBuilder.AnyOf(filters);

	public static ComponentsFilterBuilder AnyOf(params Type[] components)
		=> ComponentsFilterBuilder.AnyOf(components);

	public static ComponentsFilterBuilder NoneOf(params ComponentsFilterBuilder[] filters)
		=> ComponentsFilterBuilder.NoneOf(filters);

	public static ComponentsFilterBuilder NoneOf(params Type[] components)
		=> ComponentsFilterBuilder.NoneOf(components);
}