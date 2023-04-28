using System.Data;
using ECS.Serialize.Sql;
using Macropus.CoolStuff.Collections.Pool;
using Macropus.Database.Adapter;
using Macropus.Linq;
using Macropus.Schema;

// ReSharper disable ParameterHidesMember

namespace ECS.Serialize.Deserialize.State.Impl;

class ComponentDeserializeState : IDeserializeState
{
	private static readonly StatePool Pool = StatePool.Instance;
	private static readonly Pool<ReadResult> ReadPool = Pool<ReadResult>.Instance;

	private static readonly ListPool<DataSchemaElement> DataSchemaElementListPool = ListPool<DataSchemaElement>.Instance;
	private static readonly ListPool<long?> NullableIdsListPool = ListPool<long?>.Instance;

	private DataSchema schema;
	private long componentId;
	private ReadResult? readResult;

	public ComponentDeserializeState Init(DataSchema schema, long componentId)
	{
		this.schema = schema;
		this.componentId = componentId;

		return this;
	}

	public async Task Read(IDbConnection dbConnection)
	{
		if (readResult != null)
			return;

		var tableName = schema.SchemaOf.FullName;
		if (tableName == null)
			// TODO
			throw new Exception();

		var cmd = DbCommandCache.GetReadCmd(dbConnection, tableName, schema.Elements);

		cmd.Parameters.Add(new SqliteParameter("@id", componentId));

		using var reader = await cmd.ExecuteReaderAsync();

		await reader.ReadAsync();

		readResult = SqlComponentReader.ReadComponent(reader, schema);

		FillNullComplexFields();
	}

	private void FillNullComplexFields()
	{
		if (readResult == null)
			throw new Exception();

		var complexRefs = DataSchemaElementListPool.Take();
		readResult.ComplexRefs
			.Where(w => !w.Value.HasValue)
			.Select(w => w.Key)
			.Fill(complexRefs);

		foreach (var element in complexRefs)
		{
			readResult.ReadRefs[element] = null;
			readResult.ComplexRefs.Remove(element);
		}

		complexRefs.Clear();

		readResult.ComplexCollectionsRefs
			.Where(w => w.Value == null)
			.Select(w => w.Key)
			.Fill(complexRefs);

		foreach (var element in complexRefs)
		{
			readResult.ReadRefs[element] = null;
			readResult.ComplexCollectionsRefs.Remove(element);
		}

		complexRefs.Clear();

		readResult.ComplexCollectionsRefs
			.Where(w => w.Value != null && w.Value.All(r => r == null))
			.Select(w => w.Key)
			.Fill(complexRefs);


		foreach (var element in complexRefs)
		{
			var fieldType = element.FieldInfo.FieldType.GetElementType();
			if (fieldType == null)
				// TODO
				throw new Exception();

			var arrayLength = readResult.ComplexCollectionsRefs[element]!.Count;
			var array = Array.CreateInstance(fieldType, arrayLength);

			readResult.ReadRefs[element] = array;

			readResult.ComplexCollectionsRefs.Remove(element);
		}


		DataSchemaElementListPool.Release(complexRefs);
	}

	public bool HasRefs()
	{
		if (readResult == null)
			return false;

		return readResult.ComplexRefs.Count > 0 || readResult.ComplexCollectionsRefs.Count > 0;
	}

	public IDeserializeState PopSomeRefs()
	{
		if (readResult == null)
			throw new Exception();

		if (readResult.ComplexRefs.Count != 0)
		{
			var (element, id) = readResult.ComplexRefs.First();
			readResult.ComplexRefs.Remove(element);

			if (element.Info.SubSchemaId == null)
				// TODO
				throw new Exception();

			var schema = this.schema.SubSchemas[element.Info.SubSchemaId.Value];

			return Pool.RefDeserializeStatePool.Take().Init(schema, id.Value, element);
		}

		if (readResult.ComplexCollectionsRefs.Count != 0)
		{
			var (element, ids) = readResult.ComplexCollectionsRefs.First();
			readResult.ComplexCollectionsRefs.Remove(element);
			

			if (element.Info.SubSchemaId == null)
				// TODO
				throw new Exception();

			var schema = this.schema.SubSchemas[element.Info.SubSchemaId.Value];

			var state = Pool.RefCollectionDeserializeStatePool.Take().Init(schema, ids, element);
			
			NullableIdsListPool.Release(ids);

			return state;
		}

		throw new Exception();
	}

	public void AddRef(DataSchemaElement target, object? obj)
	{
		if (readResult == null)
			throw new Exception();

		readResult.ReadRefs[target] = obj;
	}

	public object? Create()
	{
		if (readResult == null)
			throw new Exception();

		var instance = Activator.CreateInstance(schema.SchemaOf);

		foreach (var (element, value) in readResult.SimpleValues)
			element.FieldInfo.SetValue(instance, value);

		foreach (var (element, value) in readResult.ReadRefs)
			element.FieldInfo.SetValue(instance, value);


		return instance;
	}

	public void Clear()
	{
		schema = null;
		componentId = 0;

		if (readResult != null)
		{
			ReadPool.Release(readResult);
			readResult = null;
		}
	}
}