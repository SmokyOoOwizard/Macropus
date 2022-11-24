using Macropus.Interfaces.Project;

namespace Macropus.Project.Instance;

public interface IProjectInstance : IDisposable
{
	IProjectInformation ProjectInformation { get; }
}