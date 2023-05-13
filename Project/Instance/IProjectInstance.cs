using Macropus.CoolStuff;

namespace Macropus.Project.Instance;

public interface IProjectInstance : IDisposable, IInitializableAsync
{
	IObservable<EInstanceState> State { get; }
	IProjectInformation ProjectInformation { get; }
}