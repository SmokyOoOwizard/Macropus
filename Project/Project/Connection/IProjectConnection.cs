namespace Macropus.Project.Connection;

public interface IProjectConnection : IDisposable
{
    IObservable<EConnectionState> State{ get; }
    IProjectInformation ProjectInformation { get; }
}