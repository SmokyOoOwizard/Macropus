namespace Macropus.Schema;

public struct DataSchemaElement
{
	public ESchemaElementType Type;
	public bool Nullable;
	public string Name;
	public string FieldName;
	public ECollectionType? CollectionType;

	public Guid? SubSchemaId;


	public override string ToString()
	{
		return
			"{\n"
			+ $"\t Type: {Type}\n "
			+ $"\t Name: {Name}\n"
			+ $"\t FieldName: {FieldName}\n"
			+ $"\t Nullable: {Nullable}\n"
			+ $"\t Collection: {CollectionType}\n"
			+ $"\t Sub schema {SubSchemaId}\n"
			+ "}";
	}
}