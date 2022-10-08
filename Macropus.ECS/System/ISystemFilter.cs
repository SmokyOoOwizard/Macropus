using Macropus.ECS.ComponentsStorage;

namespace Macropus.ECS.System;

public interface ISystemFilter
{
	IEnumerable<Guid> Filter(IEnumerationComponents components);
}