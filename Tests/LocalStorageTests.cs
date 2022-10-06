using Macropus.Project.Storage.Impl;
using Tests.Utils;
using Xunit.Abstractions;

namespace Tests;

public class LocalStorageTests : TestsWithFiles
{
    private const string PROJECT_NAME = "CreateTest";

    public LocalStorageTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public async void CreateByNameTest()
    {
        using var localStorage = new ProjectsStorageLocal(ExecutePath);

        var projectId = await localStorage
            .CreateProjectAsync(new ProjectCreationInfo { Name = PROJECT_NAME })
            .ConfigureAwait(false);

        Assert.NotEqual(Guid.Empty, projectId);
    }

    [Fact]
    public async void CreateByPathTest()
    {
        const string ADDITIONAL_PATH = "Additional";

        using var localStorage = new ProjectsStorageLocal(ExecutePath);

        var path = Path.Combine(ExecutePath, ADDITIONAL_PATH);

        var projectId = await localStorage
            .CreateProjectByPathAsync(path, new ProjectCreationInfo { Name = PROJECT_NAME })
            .ConfigureAwait(false);

        Assert.NotEqual(Guid.Empty, projectId);
    }

    [Fact]
    public async void CreateByExistsPathTest()
    {
        const string ADDITIONAL_PATH = "Additional";

        using var localStorage = new ProjectsStorageLocal(ExecutePath);

        var path = Path.Combine(ExecutePath, ADDITIONAL_PATH);

        Directory.CreateDirectory(path);

        var projectId = await localStorage
            .CreateProjectByPathAsync(path, new ProjectCreationInfo { Name = PROJECT_NAME })
            .ConfigureAwait(false);

        Assert.NotEqual(Guid.Empty, projectId);
    }
}