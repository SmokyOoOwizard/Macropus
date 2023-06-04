using Macropus.CoolStuff;

namespace Macropus.Project.Instance;

public interface IProjectInstance : IDisposable, IInitializableAsync
{
	IProjectInformation ProjectInformation { get; }
}