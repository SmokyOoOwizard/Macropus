namespace Macropus.ECS.Systems;

public interface IFilteredSystem
{
	static abstract ComponentsFilter Filter();
}