namespace Macropus.Project.Connection;

public interface IConnectionService
{
	Task<IProjectConnection> Connect(Guid projectId);
}