using Macropus.Linq;
using Macropus.Schema.Extensions;

namespace Macropus.Schema;

public sealed class DataSchema
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
				.ToHashSet();

			var skipComplexTypes = subSchemasStorage == null;
			foreach (var schemaElement in Elements)
			{
				if (skipComplexTypes && (schemaElement.Type == ESchemaElementType.ComplexType))
					continue;

				var targetField = targetFields.FirstOrDefault(f => f.Name == schemaElement.FieldName);
				if (targetField == null)
					return false;

				var targetFieldType = targetField.FieldType.GetSchemaType();
				if (targetFieldType != schemaElement.Type)
					return false;

				DataSchemaElement targetSchemaElement;
				if (skipComplexTypes)
					targetSchemaElement = DataSchemaElement.Create(targetField);
				else
					targetSchemaElement =
						DataSchemaElement.Create(targetField, subSchemasStorage!.GetSchemaId);

				if (!schemaElement.Equals(targetSchemaElement))
					return false;

				targetFields.Remove(targetField);
			}

			return true;
		}
		catch
		{
			return false;
		}
	}
}