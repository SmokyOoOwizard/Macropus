using System.Data;
using System.Text;
using ECS.Schema;
using ECS.Schema.Extensions;
using ECS.Serialize.Extensions;
using SpanJson;

namespace ECS.Serialize.Deserialize;

internal static class SqlComponentReader
{
	public static Dictionary<DataSchemaElement, object?> ReadComponent(IDataReader reader, DataSchema schema)
	{
		var result = new Dictionary<DataSchemaElement, object?>();

		for (var i = 0; i < schema.Elements.Count; i++)
		{
			var element = schema.Elements[i];

			if (reader.IsDBNull(i))
			{
				result[element] = null;
				continue;
			}

			if (element.Info.CollectionType is ECollectionType.Array || element.Info.Type is ESchemaElementType.ComplexType)
			{
				var json = reader.GetString(i);
				var rawJson = Encoding.UTF8.GetBytes(json);

				var obj = JsonSerializer.NonGeneric.Utf8.Deserialize(rawJson, element.FieldInfo.FieldType);
				result[element] = obj;

				continue;
			}

			if (element.Info.Type.IsSimpleType())
			{
				result[element] = element.Info.Type.Read(reader, i);
			}
		}

		return result;
	}
}