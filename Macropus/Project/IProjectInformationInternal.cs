using Macropus.Interfaces.Project;

namespace Macropus.Project;

internal interface IProjectInformationInternal : IProjectInformation
{
	string Path { get; }
}