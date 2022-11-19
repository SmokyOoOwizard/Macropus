using Macropus.Interfaces.User;
using Macropus.Project.Connection;

namespace Macropus.Project.Storage;

public interface IProjectsStorage : IDisposable
{
	Task<Guid> CreateProjectAsync(IProjectCreationInfo creationInfo, CancellationToken cancellationToken = default);

	Task<IProjectConnection> OpenProjectAsync(
		Guid id,
		IUser user,
		CancellationToken cancellationToken = default
	);

	Task<bool> DeleteProjectAsync(
		Guid id,
		IUser user,
		CancellationToken cancellationToken = default
	);

	Task GetProjectInformation(Guid projectId, IUser user, CancellationToken cancellationToken = default);

	Task<Guid[]> GetExistsProjectsAsync(
		IUser user,
		CancellationToken cancellationToken = default
	);
}