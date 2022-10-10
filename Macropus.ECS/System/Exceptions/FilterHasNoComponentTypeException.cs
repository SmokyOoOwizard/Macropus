namespace Macropus.ECS.System.Exceptions;

public class FilterHasTypesWhichAreNotComponentsException : Exception
{
	public readonly Type[] NotComponentTypes;

	public FilterHasTypesWhichAreNotComponentsException(Type[] notComponentTypes)
	{
		NotComponentTypes = notComponentTypes;
	}

	public override string Message =>
		$"Filter has types which are not components\n{string.Join('\n', NotComponentTypes.Select(t => t.FullName))}";
}