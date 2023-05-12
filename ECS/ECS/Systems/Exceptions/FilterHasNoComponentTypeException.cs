using System;
using System.Linq;

namespace Macropus.ECS.Systems.Exceptions;

public class TypesAreNotComponentsException : Exception
{
	public readonly Type[] NotComponentTypes;

	public TypesAreNotComponentsException(Type[] notComponentTypes)
	{
		NotComponentTypes = notComponentTypes;
	}

	public override string Message =>
		$"Filter has types which are not components\n{string.Join('\n', NotComponentTypes.Select(t => t.FullName))}";
}