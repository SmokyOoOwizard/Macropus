using AnyOfTypes;
using Macropus.Interfaces.User;
using Macropus.Project.Connection;

namespace Macropus.Project.Storage;

internal interface IProjectsStorage : IDisposable
{
    Task<Guid> CreateProjectAsync(IProjectCreationInfo creationInfo, CancellationToken cancellationToken = default);

    Task<IProjectConnection> OpenProjectAsync(AnyOf<Guid, string> idOrName, IUser user,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteProjectAsync(AnyOf<Guid, string> idOrName, IUser user,
        CancellationToken cancellationToken = default);
}