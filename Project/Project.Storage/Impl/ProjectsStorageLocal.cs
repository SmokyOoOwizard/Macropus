using Macropus.Project.Raw;
using Macropus.Project.Raw.Impl;
using Macropus.Project.Storage.Utils;

namespace Macropus.Project.Storage.Impl;

public class ProjectsStorageLocal : IProjectsStorage
{
	private readonly string storagePath;
	private readonly RawProjectFactory rawProjectFactory;

	public ProjectsStorageLocal(string path, RawProjectFactory rawProjectFactory)
	{
		storagePath = path;
		this.rawProjectFactory = rawProjectFactory;
	}


	public async Task<Guid> CreateProjectAsync(
		IProjectCreationInfo creationInfo,
		CancellationToken cancellationToken = default
	)
	{
		return await rawProjectFactory.CreateProjectAsync(storagePath, creationInfo, cancellationToken);
	}

	public Task<bool> DeleteProjectAsync(
		Guid id,
		CancellationToken cancellationToken = default
	)
	{
		throw new NotImplementedException();
	}

	public Task GetProjectInformation(Guid projectId, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<Guid[]> GetExistsProjectsAsync(CancellationToken cancellationToken = default)
	{
		return ProjectsLocalUtils.GetProjectsAsync(storagePath, cancellationToken);
	}


	public async Task<IRawProject> OpenProjectAsync(
		Guid id,
		CancellationToken cancellationToken = default
	)
	{
		var path = await ProjectsLocalUtils.FindProjectAsync(storagePath, id,
			cancellationToken);

		return await rawProjectFactory.Open(path, cancellationToken);
	}

	public void Dispose() { }
}