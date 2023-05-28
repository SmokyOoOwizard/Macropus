namespace ECS.Schema.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Struct)]
public class NameAttribute : Attribute
{
	public readonly string Name;

	public NameAttribute(string name)
	{
		Name = name;
	}
}