using ECS.Schema;

namespace ECS.Tests.Utils.Schema;

public static class DataSchemaElementUtils
{
	public static void ElementsEqualsColdElements(
		IReadOnlyCollection<DataSchemaElement> elements,
		IReadOnlyCollection<ColdDataSchemaElement> coldElements
	)
	{
		Assert.Equal(elements.Count, coldElements.Count);
		foreach (var element in elements)
		{
			var coldElement = coldElements.FirstOrDefault(e => e.Equals(element.Info));
			Assert.NotNull(coldElement);
			Assert.Equal(element.Info, coldElement);
		}
	}
}