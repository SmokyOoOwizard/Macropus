using System.Data;
using ECS.Schema;
using ECS.Serialize.Sql;
using Macropus.CoolStuff.Collections.Pool;
using Macropus.Database.Adapter;
using Microsoft.Data.Sqlite;

// ReSharper disable ParameterHidesMember

namespace ECS.Serialize.Deserialize.State.Impl;

class ComponentDeserializeState : IDeserializeState
{
	private static readonly Pool<ReadResult> ReadPool = Pool<ReadResult>.Instance;

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
	}

	public object? Create()
	{
		if (readResult == null)
			throw new Exception();

		var instance = Activator.CreateInstance(schema.SchemaOf);

		foreach (var (element, value) in readResult.SimpleValues)
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