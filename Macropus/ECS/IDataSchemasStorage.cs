using Macropus.Schema;

namespace Macropus.ECS;

public interface IDataSchemasStorage
{
	void AddSchema(DataSchema schema);
	void AddSchema<T>(DataSchema schema);
	void AddSchema(DataSchema schema, Type type);

	bool SchemaExists(Guid id);
	bool SchemaExists<T>();
	bool SchemaExists(Type type);

	DataSchema GetSchema(Guid id);
	DataSchema GetSchema<T>();
	DataSchema GetSchema(Type type);
}