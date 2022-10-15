using System.Reflection;
using Macropus.Extensions;
using Macropus.Schema;
using Macropus.Schema.Attributes;
using Macropus.Schema.Extensions;
using Tests.Utils.Tests;
using Xunit.Abstractions;

namespace Tests.Schema.SchemaElement;

public class ValueTypesTests : TestsWrapper
{
	public ValueTypesTests(ITestOutputHelper output) : base(output) { }

	[Fact]
	public void CreateSchemaElementFromSimpleTypeTest()
	{
		var fields = typeof(SchemaElementTestsComponent).GetFields()
			.Where(t => t.IsPublic && t.FieldType.GetSchemaType().IsSimpleType())
			.ToArray();

		var subSchemas = new List<DataSchema>();
		foreach (var field in fields)
		{
			try
			{
				subSchemas.Clear();
				var element = DataSchemaUtils.CreateElement(field, subSchemas);

				Assert.NotEqual(ESchemaElementType.INVALID, element.Type);
				Assert.Equal(field.Name, element.FieldName);

				var namedField = field.GetCustomAttribute<NameAttribute>();
				if (namedField != null)
					Assert.Equal(namedField.Name, element.Name);
				else
					Assert.Equal(field.Name, element.Name);

				Assert.Empty(subSchemas);
			}
			catch (Exception ex)
			{
				Assert.Fail($"Cannot create schema element for field: {field.Name}\n{ex}");
			}
		}
	}

	[Fact]
	public void CreateSchemaElementFromComplexTypeTest()
	{
		var fields = typeof(SchemaElementTestsComponent).GetFields()
			.Where(t => t.IsPublic && !t.FieldType.GetSchemaType().IsSimpleType())
			.ToArray();

		var subSchemas = new List<DataSchema>();
		foreach (var field in fields)
		{
			try
			{
				subSchemas.Clear();
				var element = DataSchemaUtils.CreateElement(field, subSchemas);

				Assert.NotEqual(ESchemaElementType.INVALID, element.Type);
				Assert.Equal(field.Name, element.FieldName);

				var namedField = field.GetCustomAttribute<NameAttribute>();
				if (namedField != null)
					Assert.Equal(namedField.Name, element.Name);
				else
					Assert.Equal(field.Name, element.Name);

				if (field.FieldType.IsStruct())
				{
					Assert.NotNull(element.SubSchemaId);
					Assert.NotEqual(Guid.Empty, element.SubSchemaId);
					Assert.True(subSchemas.Count > 0);
				}

				if (field.FieldType.IsArray)
				{
					Assert.NotNull(element.CollectionType);
					Assert.Equal(ECollectionType.Array, element.CollectionType);
				}
			}
			catch (Exception ex)
			{
				Assert.Fail($"Cannot create schema element for field: {field.Name}\n{ex}");
			}
		}
	}
}