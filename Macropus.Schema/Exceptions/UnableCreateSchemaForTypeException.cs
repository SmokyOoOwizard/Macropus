namespace Macropus.Schema.Exceptions;

public class UnableCreateSchemaForTypeException : Exception
{
	private readonly Type type;

	public override string Message => $"Unable create schema for type: {type.FullName}";

	public UnableCreateSchemaForTypeException(Type type)
	{
		this.type = type;
	}
}