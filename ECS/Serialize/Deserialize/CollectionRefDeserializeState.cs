using System.Data;
using Macropus.Linq;
using Macropus.Schema;

namespace Macropus.ECS.Serialize.Deserialize;

class CollectionRefDeserializeState : ITargetDeserializeState
{
	public DataSchema Schema;
	public List<object?> ReadComponents;
	public Stack<long?> Components;

	public DataSchemaElement Target { get; private set; }

	public CollectionRefDeserializeState Init(
		DataSchema schema,
		List<long?> components,
		DataSchemaElement target
	)
	{
		Schema = schema;

		ReadComponents = new();

		Components = new();

		components.Fill(Components);
		Target = target;

		return this;
	}

	public Task Read(IDbConnection dbConnection)
	{
		return Task.CompletedTask;
	}

	public bool HasRefs()
	{
		return Components.Count > 0;
	}

	public IDeserializeState PopSomeRefs()
	{
		long componentId = 0;
		do
		{
			if (Components.Count == 0)
				throw new Exception();

			var id = Components.Pop();
			if (id == null)
			{
				ReadComponents.Add(null);
				continue;
			}

			componentId = id.Value;
			break;
		} while (Components.Count > 0);

		return new RefComponentDeserializeState().Init(Schema, componentId, Target);
	}

	public void AddRef(DataSchemaElement target, object? obj)
	{
		ReadComponents.Add(obj);

		while (Components.Count > 0)
		{
			var id = Components.Peek();
			if (id != null)
				break;

			Components.Pop();
			ReadComponents.Add(null);
		}
	}

	public object? Create()
	{
		return ReadComponents;
	}

	public void Clear() { }
}