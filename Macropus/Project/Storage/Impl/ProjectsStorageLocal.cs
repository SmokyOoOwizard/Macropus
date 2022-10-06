using AnyOfTypes;
using Macropus.Interfaces.User;
using Macropus.Project.Connection;
using Macropus.Project.Connection.Impl;
using Macropus.Project.Provider.Impl;

namespace Macropus.Project.Storage.Impl;

public class ProjectsStorageLocal : IProjectsStorageLocal
{
    private readonly string storagePath;

    public ProjectsStorageLocal(string storagePath)
    {
        this.storagePath = storagePath;
    }


    public Task<Guid> CreateProjectAsync(IProjectCreationInfo creationInfo,
        CancellationToken cancellationToken = default)
    {
        return CreateProjectByPathAsync(GetPath(creationInfo.Name), creationInfo, cancellationToken);
    }

    public async Task<Guid> CreateProjectByPathAsync(string path, IProjectCreationInfo creationInfo,
        CancellationToken cancellationToken = default)
    {
        await ProjectUtils.CreateProject(path, creationInfo, cancellationToken);

        var projectInfo = await ProjectUtils.TryGetProjectInfo(path, cancellationToken);

        if (projectInfo == null)
            // TODO throw failed read project info
            throw new Exception();

        return projectInfo.Id;
    }

    public Task<bool> DeleteProjectAsync(AnyOf<Guid, string> idOrName, IUser user,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }


    public async Task<IProjectConnection> OpenProjectAsync(AnyOf<Guid, string> idOrName, IUser user,
        CancellationToken cancellationToken = default)
    {
        if (idOrName.IsSecond) return await OpenProjectByPathAsync(GetPath(idOrName.Second), user, cancellationToken);

        var path = await ProjectsStorageLocalUtils.FindProjectAsync(GetPath(string.Empty), idOrName.First,
            cancellationToken);
        return await OpenProjectByPathAsync(path, user, cancellationToken);
    }

    public async Task<IProjectConnection> OpenProjectByPathAsync(string path, IUser user,
        CancellationToken cancellationToken = default)
    {
        var projInstance = await ProjectProvider.Create(path, cancellationToken);

        return await ProjectConnection.Create(projInstance, cancellationToken)
            .ConfigureAwait(false);
    }

    public Task<bool> DeleteProjectByPathAsync(string path, IUser user,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private string GetPath(string name)
    {
        return Path.Combine(storagePath, name);
    }

    public void Dispose()
    {
    }
}