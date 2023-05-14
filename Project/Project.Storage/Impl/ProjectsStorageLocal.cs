using Macropus.Project.Raw;
using Macropus.Project.Raw.Impl;
using Macropus.Project.Storage.Raw;
using Macropus.Project.Storage.Raw.Impl;
using Macropus.Project.Storage.Utils;

namespace Macropus.Project.Storage.Impl;

public class ProjectsStorageLocal : IProjectsStorage
{
	private readonly string storagePath;
	private readonly RawProjectFactory rawProjectFactory;

	public ProjectsStorageLocal(string path, RawProjectFactory rawProjectFactory)
	{
		this.storagePath = path;
		this.rawProjectFactory = rawProjectFactory;
	}


	public async Task<Guid> CreateProjectAsync(
		IProjectCreationInfo creationInfo,
		CancellationToken cancellationToken = default
	)
	{
		var path = GetPath(creationInfo.Name);

		await ProjectUtils.CreateProject(path, creationInfo, cancellationToken);

		var projectInfo = await ProjectUtils.TryGetProjectInfo(path, cancellationToken);

		if (projectInfo == null)
			// TODO throw failed read project info
			throw new Exception();

		return projectInfo.Id;
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
		var path = await ProjectsLocalUtils.FindProjectAsync(GetPath(string.Empty), id,
			cancellationToken);

		return await rawProjectFactory.Create(path, cancellationToken);
	}

	private string GetPath(string name)
	{
		return Path.Combine(storagePath, name);
	}

	public void Dispose() { }
}