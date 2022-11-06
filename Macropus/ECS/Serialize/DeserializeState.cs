using Macropus.Linq;
using Macropus.Schema;

namespace Macropus.ECS.Serialize;

struct DeserializeState
{
	public readonly DataSchema Schema;
	public readonly DataSchemaElement? element;
	public readonly long ComponentId;

	public List<KeyValuePair<DataSchemaElement, object?>> ReadValues;
	public List<DataSchemaElement> UnreadValues;
	public Stack<KeyValuePair<DataSchemaElement, long?>> Refs;

	public DeserializeState(DataSchema schema, long componentId)
	{
		Schema = schema;
		ComponentId = componentId;

		ReadValues = new();

		UnreadValues = new();
		schema.Elements.Fill(UnreadValues);

		Refs = new();

		element = null;
	}

	public DeserializeState(DataSchema schema, DataSchemaElement element, long componentId)
	{
		Schema = schema;
		this.element = element;
		ComponentId = componentId;

		ReadValues = new();

		UnreadValues = new();
		schema.Elements.Fill(UnreadValues);

		Refs = new();
	}
}