using Macropus.Schema;

namespace Macropus.ECS;

public static class DataSchemaExtensions
{
	public static DataSchema[] GetSubSchemas(this DataSchema schema, IDataSchemasStorage schemasStorage)
	{
		var to = new Stack<Guid>(schema.SubSchemas);
		var subSchemas = new Dictionary<Guid, DataSchema>();
		do
		{
			var subSchemaId = to.Pop();
			if (subSchemas.ContainsKey(subSchemaId))
				continue;

			var subSchema = schemasStorage.GetSchema(subSchemaId);
			subSchemas.Add(subSchema.Id, subSchema);
			foreach (var id in subSchema.SubSchemas)
				to.Push(id);
		} while (to.Count > 0);

		return subSchemas.Values.ToArray();
	}
}