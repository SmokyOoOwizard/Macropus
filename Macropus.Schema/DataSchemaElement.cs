namespace Macropus.Schema;

public struct DataSchemaElement
{
	public ESchemaElementType Type;
	public string Name;
	public string FieldName;
	public ECollectionType? CollectionType;

	public Guid? SubSchemaId;
}