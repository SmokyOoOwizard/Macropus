using System.Data;
using System.Text.Json.Nodes;
using Macropus.ECS.Serialize.Extensions;
using Macropus.Schema;
using Macropus.Schema.Extensions;

namespace Macropus.ECS.Serialize.Sql;

static class SqlComponentReader
{
	// ReSharper disable once CognitiveComplexity
	public static ReadResult ReadComponent(
		IDataReader reader,
		DataSchema schema
	)
	{
		var result = ReadResult.Init();

		for (var i = 0; i < schema.Elements.Count; i++)
		{
			var element = schema.Elements[i];

			if (element.Info.CollectionType is ECollectionType.Array)
			{
				if (element.Info.Type.IsSimpleType())
					ReadSimpleArray(ref result, reader, i, element);
				else
					ReadComplexArray(ref result, reader, i, element);

				continue;
			}

			if (element.Info.Type.IsSimpleType())
			{
				if (reader.IsDBNull(i))
					result.SimpleValues[element] = null;
				else
					result.SimpleValues[element] = element.Info.Type.Read(reader, i);
				
				continue;
			}

			if (reader.IsDBNull(i))
				result.ComplexRefs[element] = null;
			else
				result.ComplexRefs[element] = reader.GetInt64(i);
		}

		return result;
	}

	private static void ReadSimpleArray(
		ref ReadResult result,
		IDataReader reader,
		int i,
		DataSchemaElement element
	)
	{
		if (reader.IsDBNull(i))
		{
			result.SimpleValues[element] = null;
			return;
		}

		var rawArray = reader.GetString(i);
		var jsonArray = JsonNode.Parse(rawArray)!.AsArray();

		var fieldType = element.FieldInfo.FieldType.GetElementType();
		if (fieldType == null)
			// TODO
			throw new Exception();

		var array = Array.CreateInstance(fieldType, jsonArray.Count);

		for (var j = 0; j < array.Length; j++)
			array.SetValue(element.Info.Parse(jsonArray[j]?.ToString()), j);

		result.SimpleValues[element] = array;
	}

	private static void ReadComplexArray(
		ref ReadResult result,
		IDataReader reader,
		int i,
		DataSchemaElement element
	)
	{
		if (reader.IsDBNull(i))
		{
			result.ComplexCollectionsRefs[element] = null;
			return;
		}

		var rawArray = reader.GetString(i);
		var jsonArray = JsonNode.Parse(rawArray)!.AsArray();

		var ids = new List<long?>();

		foreach (var arrayElement in jsonArray)
		{
			var rawElement = arrayElement?.ToString();

			if (string.IsNullOrWhiteSpace(rawElement))
				ids.Add(null);
			else
				ids.Add(long.Parse(rawElement));
		}

		result.ComplexCollectionsRefs[element] = ids;
	}
}