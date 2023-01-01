using System.Collections;
using System.Data;
using Macropus.Database.Adapter;
using Macropus.ECS.Serialize.Sql;
using Macropus.Schema;
using Macropus.Schema.Extensions;
using Microsoft.Data.Sqlite;

namespace Macropus.ECS.Serialize.Deserialize;

class ComponentDeserializeState : IDeserializeState
{
	public DataSchema Schema;
	public long ComponentId;
	public ReadResult? ReadResult;

	public ComponentDeserializeState Init(DataSchema schema, long componentId)
	{
		Schema = schema;
		ComponentId = componentId;

		return this;
	}

	public async Task Read(IDbConnection dbConnection)
	{
		if (ReadResult.HasValue)
			return;

		var tableName = Schema.SchemaOf.FullName;
		if (tableName == null)
			// TODO
			throw new Exception();

		var cmd = DbCommandCache.GetReadCmd(dbConnection, tableName, Schema.Elements);

		cmd.Parameters.Add(new SqliteParameter("@id", ComponentId));

		using var reader = await cmd.ExecuteReaderAsync();

		await reader.ReadAsync();

		ReadResult = SqlComponentReader.ReadComponent(reader, Schema);

		FillNullComplexFields();
	}

	private void FillNullComplexFields()
	{
		if (!ReadResult.HasValue)
			throw new Exception();

		var result = ReadResult.Value;

		var complexRefs = result.ComplexRefs
			.Where(w => !w.Value.HasValue)
			.Select(w => w.Key)
			.ToArray();

		foreach (var element in complexRefs)
		{
			result.ReadRefs[element] = null;
			result.ComplexRefs.Remove(element);
		}

		var complexCollections = result.ComplexCollectionsRefs
			.Where(w => w.Value == null || w.Value.All(r => r == null))
			.ToArray();

		foreach (var (element, value) in complexCollections)
		{
			if (value == null)
				result.ReadRefs[element] = null;
			else
			{
				var fieldType = element.FieldInfo.FieldType.GetElementType();
				if (fieldType == null)
					// TODO
					throw new Exception();

				var array = Array.CreateInstance(fieldType, value.Count);

				result.ReadRefs[element] = array;
			}

			result.ComplexCollectionsRefs.Remove(element);
		}
	}

	public bool HasRefs()
	{
		if (ReadResult == null)
			return false;

		var result = ReadResult.Value;

		return result.ComplexRefs.Count > 0 || result.ComplexCollectionsRefs.Count > 0;
		//return result.ComplexRefs.Count > 0;
	}

	public IDeserializeState PopSomeRefs()
	{
		if (ReadResult == null)
			throw new Exception();

		var result = ReadResult.Value;
		if (result.ComplexRefs.Count != 0)
		{
			var (element, id) = result.ComplexRefs.First();
			result.ComplexRefs.Remove(element);

			if (element.Info.SubSchemaId == null)
				// TODO
				throw new Exception();

			var schema = Schema.SubSchemas[element.Info.SubSchemaId.Value];

			return new RefComponentDeserializeState().Init(schema, id.Value, element);
		}

		if (result.ComplexCollectionsRefs.Count != 0)
		{
			var (element, ids) = result.ComplexCollectionsRefs.First();
			result.ComplexCollectionsRefs.Remove(element);

			if (element.Info.SubSchemaId == null)
				// TODO
				throw new Exception();

			var schema = Schema.SubSchemas[element.Info.SubSchemaId.Value];

			return new CollectionRefDeserializeState().Init(schema, ids, element);
		}

		throw new Exception();
	}

	public void AddRef(DataSchemaElement target, object? obj)
	{
		if (!ReadResult.HasValue)
			throw new Exception();

		var result = ReadResult.Value;

		result.ReadRefs[target] = obj;
	}

	public object? Create()
	{
		if (!ReadResult.HasValue)
			throw new Exception();

		return Create(ReadResult.Value, Schema);
	}
	public static object? Create(ReadResult readResult, DataSchema schema)
	{

		var instance = Activator.CreateInstance(schema.SchemaOf);

		foreach (var (element, value) in readResult.SimpleValues)
			element.FieldInfo.SetValue(instance, value);

		foreach (var (element, value) in readResult.ReadRefs)
		{
			if (value == null)
			{
				element.FieldInfo.SetValue(instance, value);
				continue;
			}

			if (!element.Info.Type.IsSimpleType() && element.Info.CollectionType is ECollectionType.Array)
			{
				if (value is not IList collection)
					throw new Exception(element.FieldInfo.Name);

				var fieldType = element.FieldInfo.FieldType.GetElementType();
				if (fieldType == null)
					// TODO
					throw new Exception();

				var array = Array.CreateInstance(fieldType, collection.Count);

				for (var j = 0; j < array.Length; j++)
					array.SetValue(collection[j], j);
				
				element.FieldInfo.SetValue(instance, array);
			}
			else
				element.FieldInfo.SetValue(instance, value);
		}


		return instance;
	}


	public void Clear()
	{
		Schema = null;
		ComponentId = 0;
		ReadResult = null;
	}
}