namespace Odin.ECS;

internal interface IECSContextsInternal : IECSContexts
{
	Task TickAsync();
}