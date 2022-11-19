using Macropus.Interfaces.Project;

namespace Macropus.Project;

public interface IProjectInformationInternal : IProjectInformation
{
	string Path { get; }
}