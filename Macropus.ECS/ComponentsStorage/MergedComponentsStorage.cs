using Macropus.ECS.Component;

namespace Macropus.ECS.ComponentsStorage;

public class MergedComponentsStorage : IReadOnlyComponentsStorage
{
	public uint ComponentsCount => Math.Max(mainStorage?.ComponentsCount ?? 0, additionalStorage?.ComponentsCount ?? 0);
	public uint EntitiesCount => Math.Max(mainStorage?.EntitiesCount ?? 0, additionalStorage?.EntitiesCount ?? 0);

	private IReadOnlyComponentsStorage? mainStorage;
	private IReadOnlyComponentsStorage? additionalStorage;

	public void SetStorages(IReadOnlyComponentsStorage main, IReadOnlyComponentsStorage additional)
	{
		mainStorage = main;
		additionalStorage = additional;
	}

	public bool HasComponent<T>(Guid entityId) where T : struct, IComponent
	{
		return mainStorage.HasComponent<T>(entityId) || additionalStorage.HasComponent<T>(entityId);
	}

	public T GetComponent<T>(Guid entityId) where T : struct, IComponent
	{
		if (mainStorage.HasComponent<T>(entityId))
			return mainStorage.GetComponent<T>(entityId);

		return additionalStorage.GetComponent<T>(entityId);
	}

	public IEnumerable<Guid> GetEntities()
	{
		var mainEntities = mainStorage.GetEntities();
		var additionalEntities = additionalStorage.GetEntities();

		var passedEntities = new HashSet<Guid>();

		foreach (var entity in mainEntities)
		{
			if (passedEntities.Add(entity)) yield return entity;
		}

		foreach (var entity in additionalEntities)
		{
			if (passedEntities.Add(entity)) yield return entity;
		}
	}
}