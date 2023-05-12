using System;
using Macropus.ECS.Component.Storage;

namespace Macropus.ECS.Component.Trigger;

public readonly struct ComponentsTrigger
{
	private readonly Func<Guid, IReadOnlyComponentsStorage, IReadOnlyComponentsChangesStorage, bool> filter;

	internal ComponentsTrigger(Func<Guid, IReadOnlyComponentsStorage, IReadOnlyComponentsChangesStorage, bool> filter)
	{
		this.filter = filter;
	}

	public bool Filter(Guid entityId, IReadOnlyComponentsStorage storage, IReadOnlyComponentsChangesStorage changes)
	{
		return filter.Invoke(entityId, storage, changes);
	}

	public static ComponentsTriggerBuilder AllOf(params ComponentsTriggerBuilder[] filters)
		=> new(EComponentsTriggerType.All, filters);

	public static ComponentsTriggerBuilder AllOf(params TriggerDefine[] components)
		=> new(EComponentsTriggerType.All, components);

	public static ComponentsTriggerBuilder AnyOf(params ComponentsTriggerBuilder[] filters)
		=> new(EComponentsTriggerType.Any, filters);

	public static ComponentsTriggerBuilder AnyOf(params TriggerDefine[] components)
		=> new(EComponentsTriggerType.Any, components);

	public static ComponentsTriggerBuilder NoneOf(params ComponentsTriggerBuilder[] filters)
		=> new(EComponentsTriggerType.None, filters);

	public static ComponentsTriggerBuilder NoneOf(params TriggerDefine[] components)
		=> new(EComponentsTriggerType.None, components);
}