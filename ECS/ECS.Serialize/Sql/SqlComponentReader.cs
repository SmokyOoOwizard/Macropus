using System.Data;
using System.Text;
using ECS.Schema;
using ECS.Schema.Extensions;
using ECS.Serialize.Extensions;
using Macropus.CoolStuff.Collections.Pool;
using SpanJson;

namespace ECS.Serialize.Sql;

static class SqlComponentReader
{
	private static readonly Pool<ReadResult> ReadPool = Pool<ReadResult>.Instance;

	// ReSharper disable once CognitiveComplexity
	public static ReadResult ReadComponent(
		IDataReader reader,
		DataSchema schema
	)
	{
		var result = ReadPool.Take().Init();

		for (var i = 0; i < schema.Elements.Count; i++)
		{
			var element = schema.Elements[i];

			if (reader.IsDBNull(i))
			{
				result.SimpleValues[element] = null;
				continue;
			}

			if (element.Info.CollectionType is ECollectionType.Array || element.Info.Type is ESchemaElementType.ComplexType)
			{
				var json = reader.GetString(i);
				var rawJson = Encoding.UTF8.GetBytes(json);

				var obj = JsonSerializer.NonGeneric.Utf8.Deserialize(rawJson, element.FieldInfo.FieldType);
				result.SimpleValues[element] = obj;

				continue;
			}

			if (element.Info.Type.IsSimpleType())
			{
				result.SimpleValues[element] = element.Info.Type.Read(reader, i);
			}
		}

		return result;
	}
}