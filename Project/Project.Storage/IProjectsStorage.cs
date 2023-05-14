using Macropus.Project.Storage.Raw;

namespace Macropus.Project.Storage;

public interface IProjectsStorage : IDisposable
{
	Task<Guid> CreateProjectAsync(IProjectCreationInfo creationInfo, CancellationToken cancellationToken = default);

	Task<IRawProject> OpenProjectAsync(Guid id, CancellationToken cancellationToken = default);

	Task<bool> DeleteProjectAsync(Guid id, CancellationToken cancellationToken = default);

	Task GetProjectInformation(Guid projectId, CancellationToken cancellationToken = default);

	Task<Guid[]> GetExistsProjectsAsync(CancellationToken cancellationToken = default);
}