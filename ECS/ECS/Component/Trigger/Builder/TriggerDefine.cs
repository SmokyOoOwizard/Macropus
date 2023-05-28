namespace Macropus.ECS.Component.Trigger.Builder;

public readonly struct TriggerDefine
{
	public readonly Type componentType;
	public readonly ETriggerType triggerType;

	public TriggerDefine(Type componentType, ETriggerType triggerType)
	{
		this.componentType = componentType;
		this.triggerType = triggerType;
	}

	public static implicit operator TriggerDefine(Type type)
	{
		return new(type, ETriggerType.Any);
	}
}