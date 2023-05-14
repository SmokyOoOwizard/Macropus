using Macropus.Extensions;
using Macropus.Project.Impl;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Macropus.Project.Raw;

internal static class RawProjectUtils
{
	public static async Task CreateProject(
		string path,
		IProjectCreationInfo creationInfo,
		CancellationToken cancellationToken = default
	)
	{
		if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

		if (Directory.Exists(path))
		{
			if (DirectoryExtensions.DirectoryNotEmpty(path))
				// TODO: directory must be empty
				throw new Exception();
		}
		else
		{
			Directory.CreateDirectory(path);
		}
		// TODO: validate creation info

		var serializer = new SerializerBuilder()
			.WithNamingConvention(CamelCaseNamingConvention.Instance)
			.Build();

		var projInfo = new ProjectInformationInternal
		{
			Id = Guid.NewGuid(),
			Name = creationInfo.Name
		};

		await using var fs = new FileStream(Path.Combine(path, ProjectPaths.PROJECT_FILE_NAME), FileMode.Create);
		await using var sw = new StreamWriter(fs);

		var buffer = serializer.Serialize(projInfo).AsMemory();
		await sw.WriteLineAsync(buffer, cancellationToken);
	}
}