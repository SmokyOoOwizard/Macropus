using System.Reactive.Disposables;
using Macropus.Interfaces.Project;
using Macropus.Project.Provider;

namespace Macropus.Project.Connection.Impl;

internal class ProjectConnection : IProjectConnection
{
    private readonly IProjectProvider project;

    public IProjectInformation ProjectInformation => project.ProjectInformation;

    public ProjectConnection(IProjectProvider project)
    {
        this.project = project;
    }

    public void Dispose()
    {
        project.Dispose();
    }

    public static async Task<IProjectConnection> Create(IProjectProvider projectInstance,
        CancellationToken cancellationToken = default)
    {
        var disposable = new CompositeDisposable(Disposable.Empty);

        try
        {
            disposable.Add(projectInstance);

            return new ProjectConnection(projectInstance);
        }
        catch
        {
            disposable.Dispose();
            throw;
        }
    }
}