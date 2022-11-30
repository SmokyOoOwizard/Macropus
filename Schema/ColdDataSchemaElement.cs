namespace Macropus.Schema;

public struct ColdDataSchemaElement
{
	public ESchemaElementType Type;
	public bool Nullable;
	public string Name;
	public string FieldName;
	public ECollectionType? CollectionType;

	public uint? SubSchemaId;
}