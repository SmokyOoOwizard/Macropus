using Macropus.Project.Storage.Impl;
using Xunit.Abstractions;

namespace Tests.Utils.Tests;

public abstract class TestsWithProjectsStorage : TestsWithFiles
{
    public readonly ProjectsStorageLocal ProjectStorage;

    public TestsWithProjectsStorage(ITestOutputHelper output) : base(output)
    {
        //ProjectStorage = new ProjectsStorageLocal(ExecutePath);
    }
}