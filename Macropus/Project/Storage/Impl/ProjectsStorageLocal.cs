using Macropus.Interfaces.User;
using Macropus.Project.Connection;
using Macropus.Project.Connection.Impl;
using Macropus.Project.Provider.Impl;

namespace Macropus.Project.Storage.Impl;

public class ProjectsStorageLocal : IProjectsStorage
{
	private readonly string storagePath;
	private readonly ProjectProviderFactory projectFactory;

	public ProjectsStorageLocal(string storagePath, ProjectProviderFactory projectFactory)
	{
		this.storagePath = storagePath;
		this.projectFactory = projectFactory;
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
		IUser user,
		CancellationToken cancellationToken = default
	)
	{
		throw new NotImplementedException();
	}

	public Task GetProjectInformation(Guid projectId, IUser user, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<Guid[]> GetExistsProjectsAsync(IUser user, CancellationToken cancellationToken = default)
	{
		return ProjectsLocalUtils.GetProjectsAsync(storagePath, cancellationToken);
	}


	public async Task<IProjectConnection> OpenProjectAsync(
		Guid id,
		IUser user,
		CancellationToken cancellationToken = default
	)
	{
		var path = await ProjectsLocalUtils.FindProjectAsync(GetPath(string.Empty), id,
			cancellationToken);

		var projInstance = await projectFactory.Create(path, cancellationToken);

		return await ProjectConnection.Create(projInstance, cancellationToken)
			.ConfigureAwait(false);
	}

	private string GetPath(string name)
	{
		return Path.Combine(storagePath, name);
	}

	public void Dispose() { }
}