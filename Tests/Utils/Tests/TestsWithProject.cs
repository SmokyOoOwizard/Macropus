using Macropus.Interfaces.User;
using Macropus.Project.Connection;
using Macropus.Project.Storage;
using Macropus.Project.Storage.Impl;
using Xunit.Abstractions;

namespace Tests.Utils.Tests;

public abstract class TestsWithProject : TestsWithProjectsStorage, IAsyncLifetime
{
    public virtual IProjectCreationInfo ProjectCreateInfo { get; } = new ProjectCreationInfo() { Name = "TestProject" };

    public abstract IUser ConnectionUser { get; }

    public IProjectConnection ProjectConnection { get; private set; } = null!;

    protected TestsWithProject(ITestOutputHelper output) : base(output)
    {
    }

    public virtual async Task InitializeAsync()
    {
        var id = await ProjectStorage.CreateProjectAsync(ProjectCreateInfo).ConfigureAwait(false);

        //ProjectConnection = await ProjectStorage.OpenProjectAsync(id, ConnectionUser).ConfigureAwait(false);
    }

    public virtual Task DisposeAsync()
    {
        ProjectConnection.Dispose();

        return Task.CompletedTask;
    }
}