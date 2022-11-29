using Macropus.Interfaces.User;

namespace Macropus.Project.Connection;

public interface IConnectionService
{
	Task<IProjectConnection> Connect(IUser user, Guid projectId);
}