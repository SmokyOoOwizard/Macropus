using Macropus.Interfaces.User;
using Macropus.Project.Connection;

namespace Macropus.Project.Storage;

internal interface IProjectsStorageLocal : IProjectsStorage
{
    Task<Guid> CreateProjectByPathAsync(string path, IProjectCreationInfo creationInfo,
        CancellationToken cancellationToken = default);

    Task<IProjectConnection> OpenProjectByPathAsync(string path, IUser user,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteProjectByPathAsync(string path, IUser user, CancellationToken cancellationToken = default);
}