namespace ECS.Schema.Exceptions;

public class NullIdFactoryException : Exception
{
	public override string Message =>
		$"Unable create {nameof(DataSchemaElement)} for complex type without schema id factory";
}