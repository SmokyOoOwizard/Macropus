namespace Macropus.Schema.Exceptions;

public class UnableCreateDataSchemaElementWithoutIdFactoryException : Exception
{
	public override string Message =>
		$"Unable create {nameof(DataSchemaElement)} for complex type without schema id factory";
}