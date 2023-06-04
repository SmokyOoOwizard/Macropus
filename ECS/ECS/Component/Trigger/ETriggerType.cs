namespace Macropus.ECS.Component.Trigger;

[Flags]
public enum ETriggerType
{
	Added = 0b001,
	Removed = 0b010,
	Replaced = 0b100,
	AddedOrRemoved = Added | Removed,
	AddedOrReplaced = Added | Replaced,
	RemovedOrReplaced = Removed | Replaced,
	Any = Added | Removed | Replaced
}