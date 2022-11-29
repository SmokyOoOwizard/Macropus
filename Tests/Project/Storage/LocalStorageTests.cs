using Macropus.Project.Storage.Impl;
using Tests.Utils;
using Tests.Utils.Tests;
using Xunit.Abstractions;

namespace Tests.Project.Storage;

public class LocalStorageTests : TestsWithProjectsStorage
{
	private const string PROJECT_NAME = "CreateTest";

	public LocalStorageTests(ITestOutputHelper output) : base(output) { }

	[Fact]
	public async void CreateByNameTest()
	{
		var projectId = await ProjectStorage
			.CreateProjectAsync(new ProjectCreationInfo { Name = PROJECT_NAME })
			.ConfigureAwait(false);

		Assert.NotEqual(Guid.Empty, projectId);
	}

	[Fact]
	public async void CreateByPathTest()
	{
		const string ADDITIONAL_PATH = "Additional";

		var path = Path.Combine(ExecutePath, ADDITIONAL_PATH);

		//var projectId = await ProjectStorage
		//	.CreateProjectByPathAsync(path, new ProjectCreationInfo { Name = PROJECT_NAME })
		//	.ConfigureAwait(false);
		
		//Assert.NotEqual(Guid.Empty, projectId);
	}

	[Fact]
	public async void CreateByExistsPathTest()
	{
		const string ADDITIONAL_PATH = "Additional";

		var path = Path.Combine(ExecutePath, ADDITIONAL_PATH);

		Directory.CreateDirectory(path);

		// var projectId = await ProjectStorage
		// 	.CreateProjectByPathAsync(path, new ProjectCreationInfo { Name = PROJECT_NAME })
		// 	.ConfigureAwait(false);
		//
		// Assert.NotEqual(Guid.Empty, projectId);
	}
}