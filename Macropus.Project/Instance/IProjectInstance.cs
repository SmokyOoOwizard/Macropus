using Macropus.Interfaces;
using Macropus.Interfaces.Project;
using Macropus.Project.Instance.Impl;

namespace Macropus.Project.Instance;

public interface IProjectInstance : IDisposable, IInitializableAsync
{
	IObservable<EInstanceState> State { get; }
	IProjectInformation ProjectInformation { get; }
}