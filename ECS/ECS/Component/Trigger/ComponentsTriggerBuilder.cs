namespace Macropus.ECS.Component.Trigger;

public readonly struct ComponentsTriggerBuilder
{
	public ComponentsTriggerBuilder(EComponentsTriggerType type, params Type[] filterComponents) { }

	public ComponentsTriggerBuilder(EComponentsTriggerType type, params ComponentsTriggerBuilder[] subFilters) { }

	public static ComponentsTriggerBuilder AllOf(params ComponentsTriggerBuilder[] filters)
		=> new(EComponentsTriggerType.All, filters);

	public static ComponentsTriggerBuilder AllOf(params Type[] components)
		=> new(EComponentsTriggerType.All, components);

	public static ComponentsTriggerBuilder AnyOf(params ComponentsTriggerBuilder[] filters)
		=> new(EComponentsTriggerType.Any, filters);

	public static ComponentsTriggerBuilder AnyOf(params Type[] components)
		=> new(EComponentsTriggerType.Any, components);

	public static ComponentsTriggerBuilder NoneOf(params ComponentsTriggerBuilder[] filters)
		=> new(EComponentsTriggerType.None, filters);

	public static ComponentsTriggerBuilder NoneOf(params Type[] components)
		=> new(EComponentsTriggerType.None, components);
}