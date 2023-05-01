using System;

namespace Macropus.ECS.Component.Trigger;

public static class TypeExtensions
{
	public static TriggerDefine Added(this Type type)
		=> new(type, ETriggerType.Added);

	public static TriggerDefine AddedOrRemoved(this Type type)
		=> new(type, ETriggerType.AddedOrRemoved);

	public static TriggerDefine AddedOrReplaced(this Type type)
		=> new(type, ETriggerType.AddedOrReplaced);

	public static TriggerDefine Removed(this Type type)
		=> new(type, ETriggerType.Removed);

	public static TriggerDefine RemovedOrReplaced(this Type type)
		=> new(type, ETriggerType.RemovedOrReplaced);

	public static TriggerDefine Replaced(this Type type)
		=> new(type, ETriggerType.Replaced);

	public static TriggerDefine Any(this Type type)
		=> new(type, ETriggerType.Any);
}