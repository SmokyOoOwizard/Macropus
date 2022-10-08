namespace Macropus.ECS.ComponentsStorage;

public interface IEnumerationComponents : IReadOnlyDictionary<string, IEnumerable<Guid>>
{
	IEnumerable<Guid> GetEntities();
}