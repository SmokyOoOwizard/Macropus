using Macropus.Project.Raw.Impl;

namespace Macropus.Project.Storage.Utils;

internal static class ProjectsLocalUtils
{
	public static async Task<string> FindProjectAsync(
		string directory,
		Guid id,
		CancellationToken cancellationToken = default
	)
	{
		if (!Directory.Exists(directory))
			throw new(); // TODO throw directory not found

		var subDirectories = Directory.GetDirectories(directory);
		foreach (var subDirectory in subDirectories)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var projInfo = await RawProjectFactory.TryGetProjectInfo(subDirectory, cancellationToken);
			if (projInfo == null)
				continue;


			if (projInfo.Id == id) return subDirectory;
		}

		throw new(); // TODO throw project not find
	}

	public static async Task<Guid[]> GetProjectsAsync(
		string directory,
		CancellationToken cancellationToken = default
	)
	{
		if (!Directory.Exists(directory))
			// TODO throw directory not found
			throw new Exception();

		var dirs = Directory.GetDirectories(directory);

		var ids = new List<Guid>();
		foreach (var dir in dirs)
		{
			var info = await RawProjectFactory.TryGetProjectInfo(dir, cancellationToken).ConfigureAwait(false);
			if (info != null)
				ids.Add(info.Id);
		}

		return ids.ToArray();
	}
}