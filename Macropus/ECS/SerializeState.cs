using System.Collections;
using Macropus.Schema;

namespace Macropus.ECS;

internal struct SerializeState
{
	public DataSchema Schema;
	public object? Value;
	public List<DataSchemaElement> Unprocessed;
	public List<int?>? TargetCollection;
	public Stack<object?> Processed;

	public SerializeState(DataSchema schema, object value)
	{
		Schema = schema;
		Value = value;

		Unprocessed = schema.Elements.Where(e => e.Info.Type == ESchemaElementType.ComplexType)
			.ToList();

		TargetCollection = null;
		Processed = new Stack<object?>();
	}

	public void ProcessUnprocessed(Stack<SerializeState> stack)
	{
		var unprocessed = Unprocessed.Last();
		var unprocessedSchema = Schema.SubSchemas[unprocessed.Info.SubSchemaId!.Value];

		if (unprocessed.Info.CollectionType is ECollectionType.Array)
		{
			var ids = new List<int?>();
			Processed.Push(ids);

			if (unprocessed.FieldInfo.GetValue(Value) is not ICollection array)
				// TODO
				throw new Exception();

			foreach (var obj in array)
			{
				var state = new SerializeState(unprocessedSchema, obj);
				state.TargetCollection = ids;

				stack.Push(state);
			}

			Unprocessed.RemoveAt(Unprocessed.Count - 1);
		}
		else
		{
			stack.Push(new SerializeState(unprocessedSchema, unprocessed.FieldInfo.GetValue(Value)!));
		}
	}
}