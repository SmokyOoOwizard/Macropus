using ECS.Db.Storage.Impl;
using ECS.Db.Tests.Utils;
using ECS.Schema;
using ECS.Serialize;
using ECS.Tests.Schema;
using LinqToDB.Data;
using Tests.Utils;
using Xunit.Abstractions;

namespace ECS.Db.Tests;

public class ComponentsStorageTests : TestsWithDatabase
{
	public ComponentsStorageTests(ITestOutputHelper output) : base(output)
	{
		DataConnection.TurnTraceSwitchOn();
		DataConnection.WriteTraceLine = (s1, s2, _) => Console.WriteLine(s1);
	}

	[Fact]
	public async Task HasComponent()
	{
		var builder = new DataSchemaBuilder();
		var schema = builder.CreateSchema<DataSchemaTestTypeComponent>();

		using var serializer = new ComponentSerializer(DbConnection);
		await serializer.CreateTablesBySchema(schema);

		var guid = Guid.NewGuid();
		await serializer.SerializeAsync(schema, guid, new DataSchemaTestTypeComponent());

		var storageInDb = new ComponentsStorageInDb(DbConnection);
		Assert.True(storageInDb.HasComponent<DataSchemaTestTypeComponent>(guid));
		Assert.False(storageInDb.HasComponent<DataSchemaSubSchemaComponent2>(guid));
	}

	[Fact]
	public async Task GetComponent()
	{
		var builder = new DataSchemaBuilder();
		var schema = builder.CreateSchema<DataSchemaTestTypeComponent>();

		using var serializer = new ComponentSerializer(DbConnection);
		await serializer.CreateTablesBySchema(schema);

		var guid = Guid.NewGuid();

		var oldComponent = DataSchemaRandomUtils.GetRandomDataSchemaTestTypeComponent();

		await serializer.SerializeAsync(schema, guid, oldComponent);

		var storageInDb = new ComponentsStorageInDb(DbConnection);
		Assert.True(storageInDb.HasComponent<DataSchemaTestTypeComponent>(guid));

		var newComponent = storageInDb.GetComponent<DataSchemaTestTypeComponent>(guid);

		DataSchemaUtils.CheckDeserializedComponent(newComponent, oldComponent);
	}

	[Fact]
	public async Task RemoveComponent()
	{
		var builder = new DataSchemaBuilder();
		var schema = builder.CreateSchema<DataSchemaTestTypeComponent>();

		using var serializer = new ComponentSerializer(DbConnection);
		await serializer.CreateTablesBySchema(schema);

		var guid = Guid.NewGuid();

		var oldComponent = DataSchemaRandomUtils.GetRandomDataSchemaTestTypeComponent();

		await serializer.SerializeAsync(schema, guid, oldComponent);

		var storageInDb = new ComponentsStorageInDb(DbConnection);
		Assert.True(storageInDb.HasComponent<DataSchemaTestTypeComponent>(guid));

		var newComponent = storageInDb.GetComponent<DataSchemaTestTypeComponent>(guid);

		DataSchemaUtils.CheckDeserializedComponent(newComponent, oldComponent);

		storageInDb.RemoveComponent<DataSchemaTestTypeComponent>(guid);

		Assert.False(storageInDb.HasComponent<DataSchemaTestTypeComponent>(guid));
	}
}