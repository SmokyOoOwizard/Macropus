namespace Macropus.ECS.Component.Trigger;

public static class ComponentsTriggerBuilderExtensions
{
	public static ComponentsTriggerBuilder AllOf(this ComponentsTriggerBuilder builder, ComponentsTriggerBuilder[] triggers)
		=> new(EComponentsTriggerType.All, builder, new(EComponentsTriggerType.All, triggers));

	public static ComponentsTriggerBuilder AllOf(this ComponentsTriggerBuilder builder, params TriggerDefine[] components)
		=> new(EComponentsTriggerType.All, builder, new(EComponentsTriggerType.All, components));

	public static ComponentsTriggerBuilder AnyOf(this ComponentsTriggerBuilder builder, ComponentsTriggerBuilder[] triggers)
		=> new(EComponentsTriggerType.All, builder, new(EComponentsTriggerType.Any, triggers));

	public static ComponentsTriggerBuilder AnyOf(this ComponentsTriggerBuilder builder, params TriggerDefine[] components)
		=> new(EComponentsTriggerType.All, builder, new(EComponentsTriggerType.Any, components));

	public static ComponentsTriggerBuilder NoneOf(this ComponentsTriggerBuilder builder, ComponentsTriggerBuilder[] triggers)
		=> new(EComponentsTriggerType.All, builder, new(EComponentsTriggerType.None, triggers));

	public static ComponentsTriggerBuilder NoneOf(this ComponentsTriggerBuilder builder, params TriggerDefine[] components)
		=> new(EComponentsTriggerType.All, builder, new(EComponentsTriggerType.None, components));
}