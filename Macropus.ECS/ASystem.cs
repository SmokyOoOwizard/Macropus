namespace Macropus.ECS;

public abstract class ASystem
{
	private readonly AEntitiesContext context;

	public ASystem(AEntitiesContext context)
	{
		this.context = context;
	}

	protected abstract void Execute(IEnumerable<IEntity> entities);
}