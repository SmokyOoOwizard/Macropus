using Macropus.ECS.Component.Storage;

namespace Macropus.ECS.Component.Trigger;

public readonly struct ComponentsTrigger
{
	private readonly Func<Guid, IReadOnlyComponentsStorage, bool> filter;

	internal ComponentsTrigger(Func<Guid, IReadOnlyComponentsStorage, bool> filter)
	{
		this.filter = filter;
	}

	public bool Filter(Guid entityId, IReadOnlyComponentsStorage storage)
	{
		return filter.Invoke(entityId, storage);
	}

	public static ComponentsTriggerBuilder AllOf(params ComponentsTriggerBuilder[] filters)
		=> ComponentsTriggerBuilder.AllOf(filters);

	public static ComponentsTriggerBuilder AllOf(params Type[] components)
		=> ComponentsTriggerBuilder.AllOf(components);

	public static ComponentsTriggerBuilder AnyOf(params ComponentsTriggerBuilder[] filters)
		=> ComponentsTriggerBuilder.AnyOf(filters);

	public static ComponentsTriggerBuilder AnyOf(params Type[] components)
		=> ComponentsTriggerBuilder.AnyOf(components);

	public static ComponentsTriggerBuilder NoneOf(params ComponentsTriggerBuilder[] filters)
		=> ComponentsTriggerBuilder.NoneOf(filters);

	public static ComponentsTriggerBuilder NoneOf(params Type[] components)
		=> ComponentsTriggerBuilder.NoneOf(components);
}