using ECS.Component.Storage.Impl;
using ECS.Schema;
using ECS.Serialize;
using ECS.Tests.Schema;
using Tests.Utils;
using Xunit.Abstractions;

namespace ECS.Db.Tests;

public class ComponentsStorageTests : TestsWithDatabase
{
	public ComponentsStorageTests(ITestOutputHelper output) : base(output) { }

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
}