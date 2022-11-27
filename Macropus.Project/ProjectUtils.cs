using Macropus.Extensions;
using Macropus.Project.Impl;
using Macropus.Project.Storage;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Macropus.Project;

internal static class ProjectUtils
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

		using (var fs = new FileStream(Path.Combine(path, ProjectPaths.PROJECT_FILE_NAME), FileMode.Create))
		{
			using var sw = new StreamWriter(fs);

			var buffer = serializer.Serialize(projInfo).AsMemory();
			await sw.WriteLineAsync(buffer, cancellationToken);
		}
	}

	public static async Task<IProjectInformationInternal?> TryGetProjectInfo(
		string path,
		CancellationToken cancellationToken = default
	)
	{
		if (!Directory.Exists(path))
			return null;

		if (!File.Exists(Path.Combine(path, ProjectPaths.PROJECT_FILE_NAME)))
			return null;

		var deserializer = new DeserializerBuilder()
			.WithNamingConvention(UnderscoredNamingConvention.Instance)
			.Build();

		await using var fs = new FileStream(Path.Combine(path, ProjectPaths.PROJECT_FILE_NAME), FileMode.Open);
		using var sr = new StreamReader(fs);

		var projInfo =
			deserializer.Deserialize<ProjectInformationInternal>(await sr.ReadToEndAsync()
				.WithCancellation(cancellationToken));

		projInfo.Path = path;

		return projInfo;
	}
}