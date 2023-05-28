using ECS.Schema;

namespace ECS.Tests.Utils.Schema;

public static class DataSchemaUtils
{
	public static void AssertEquals(this DataSchema schema, DataSchema newSchema)
	{
		var checkedSchemas = new HashSet<(DataSchema, DataSchema)>();

		var queue = new Queue<(DataSchema, DataSchema)>();
		queue.Enqueue((schema, newSchema));

		do
		{
			var equal = queue.Dequeue();
			var origin = equal.Item1;
			var clone = equal.Item2;

			Assert.Equal(origin.Elements.Count, clone.Elements.Count);

			var originElements = origin.Elements.ToArray();
			var cloneElements = clone.Elements.ToArray();
			for (int i = 0; i < originElements.Length; i++)
			{
				var originElement = originElements[i];
				var cloneElement = cloneElements[i];

				Assert.Equal(originElement.FieldInfo, cloneElement.FieldInfo);
				Assert.Equal(originElement.Info, cloneElement.Info);
			}


			Assert.Equal(origin.SubSchemas.Count, clone.SubSchemas.Count);

			foreach (var subSchema in clone.SubSchemas)
			{
				var originSchema = subSchema.Value;

				Assert.True(clone.SubSchemas.ContainsKey(subSchema.Key));

				var cloneSchema = clone.SubSchemas[subSchema.Key];

				if (!checkedSchemas.Contains((originSchema, cloneSchema)))
					queue.Enqueue((originSchema, cloneSchema));
			}


			checkedSchemas.Add((origin, clone));
		} while (queue.Count > 0);
	}
}