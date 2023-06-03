﻿using ECS.Schema;
using ECS.Serialize;
using Tests.Utils;
using Xunit.Abstractions;

namespace ECS.Tests.Schema.Serializer;

public class DatabaseTests : TestsWithDatabase
{
	public DatabaseTests(ITestOutputHelper output) : base(output) { }

	[Fact]
	public async Task CreateTableByDataSchemaTest()
	{
		var builder = new DataSchemaBuilder();
		var schema = builder.CreateSchema<DataSchemaTestTypeComponent>();

		using var serializer = new ComponentSerializer(DataConnection);
		await serializer.CreateTablesBySchema(schema);
	}

	[Fact]
	public async Task TryCreateTableByDataSchemaWithAlreadyExistsSameTableTest()
	{
		var builder = new DataSchemaBuilder();
		var schema = builder.CreateSchema<DataSchemaTestTypeComponent>();

		using var serializer = new ComponentSerializer(DataConnection);

		await serializer.CreateTablesBySchema(schema);
		await serializer.CreateTablesBySchema(schema);
	}
}