using Macropus.Interfaces.Project;

namespace Macropus.Project.Connection;

public interface IProjectConnection : IDisposable
{
    IProjectInformation ProjectInformation { get; }
}