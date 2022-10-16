using Macropus.Linq;
using Macropus.Schema.Extensions;

namespace Macropus.Schema;

public class DataSchema
{
	public readonly Guid Id;
	public readonly string Name;

	public readonly IReadOnlyCollection<DataSchemaElement> Elements;
	public readonly IReadOnlyCollection<Guid> SubSchemas;

	public DataSchema(Guid id, string name, IReadOnlyCollection<DataSchemaElement> elements)
	{
		Id = id;
		Name = name;
		Elements = elements;
		SubSchemas = elements.Select(e => e.SubSchemaId).NotNull().ToArray();
	}


	public bool IsCorrectType<T>()
	{
		return IsFullCorrectType(typeof(T), null);
	}

	public bool IsCorrectType(Type type)
	{
		return IsFullCorrectType(type, null);
	}

	public bool IsFullCorrectType<T>(IDataSchemasStorage? subSchemasStorage)
	{
		return IsFullCorrectType(typeof(T), subSchemasStorage);
	}

	public bool IsFullCorrectType(Type type, IDataSchemasStorage? subSchemasStorage)
	{
		try
		{
			var targetFields = type.GetFields()
				.Where(f => f.IsPublic && f.FieldType.FilterDataSchemaElement())
				.Select(f => new { Type = f.FieldType.GetSchemaType(), FieldInfo = f })
				.ToHashSet();

			var skipComplexTypes = subSchemasStorage == null;
			foreach (var schemaElement in Elements)
			{
				if (skipComplexTypes && (schemaElement.Type == ESchemaElementType.ComplexType))
					continue;

				var targetField = targetFields.FirstOrDefault(f => f.FieldInfo.Name == schemaElement.FieldName);
				if (targetField == null)
					return false;

				if (targetField.Type != schemaElement.Type)
					return false;

				DataSchemaElement targetSchemaElement;
				if (skipComplexTypes)
					targetSchemaElement = DataSchemaElement.Create(targetField.FieldInfo);
				else
					targetSchemaElement =
						DataSchemaElement.Create(targetField.FieldInfo, subSchemasStorage!.GetSchemaId);

				if (!schemaElement.Equals(targetSchemaElement))
					return false;
			}

			return true;
		}
		catch
		{
			return false;
		}
	}
}