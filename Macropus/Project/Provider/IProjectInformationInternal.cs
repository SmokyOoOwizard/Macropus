using Macropus.Interfaces.Project;

namespace Macropus.Project.Instance;

internal interface IProjectInformationInternal : IProjectInformation
{
    string Path { get; }
}