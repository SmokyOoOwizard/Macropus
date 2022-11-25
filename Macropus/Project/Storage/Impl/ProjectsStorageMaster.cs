using System.Collections.Concurrent;
using Delogger.Scope;
using Delogger.Scope.Log;
using Macropus.Linq;
using Macropus.Project.Raw;

namespace Macropus.Project.Storage.Impl;

public class ProjectsStorageMaster : IProjectsStorage
{
	private readonly IDScope scope;
	private readonly IDLogger logger;

	private readonly ConcurrentDictionary<IProjectsStorage, bool> storages = new();

	private IProjectsStorage? defaultStorage;

	public ProjectsStorageMaster(IDScope scope)
	{
		this.scope = scope;
		logger = scope.CreateLogger(new LoggerCreateOptions() { Tags = new[] { nameof(ProjectsStorageMaster) } });
	}

	public void AddStorage(IProjectsStorage storage)
	{
		storages.TryAdd(storage, false);
	}

	public void SetDefaultStorage(IProjectsStorage storage)
	{
		if (!storages.ContainsKey(storage))
			throw new NotImplementedException();

		defaultStorage = storage;
	}

	public void RemoveStorage(IProjectsStorage storage)
	{
		storages.TryRemove(storage, out _);
	}

	public Task<Guid> CreateProjectAsync(
		IProjectCreationInfo creationInfo,
		CancellationToken cancellationToken = default
	)
	{
		if (defaultStorage == null)
			throw new NotImplementedException();

		return defaultStorage.CreateProjectAsync(creationInfo, cancellationToken);
	}

	public async Task<IRawProject> OpenProjectAsync(
		Guid id,
		CancellationToken cancellationToken = default
	)
	{
		foreach (var storage in storages)
		{
			try
			{
				return await storage.Key.OpenProjectAsync(id, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				// TODO ignore exception not found and etc
				logger.Log(ex.ToString(), new[]
				{
					"Error",
					nameof(OpenProjectAsync)
				}, null, new KeyValuePair<string, object>[]
				{
					new("Project Id", id)
				});
			}
		}

		// TODO throw exception not found
		throw new NotImplementedException();
	}

	public async Task<bool> DeleteProjectAsync(Guid id, CancellationToken cancellationToken = default)
	{
		foreach (var storage in storages)
		{
			if (await storage.Key.DeleteProjectAsync(id, cancellationToken).ConfigureAwait(false))
				return true;
		}

		return false;
	}

	public Task GetProjectInformation(Guid projectId, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public async Task<Guid[]> GetExistsProjectsAsync(CancellationToken cancellationToken = default)
	{
		var ids = new HashSet<Guid>();
		foreach (var storage in storages)
		{
			ids.AddRange(await storage.Key.GetExistsProjectsAsync(cancellationToken).ConfigureAwait(false));
		}

		return ids.ToArray();
	}

	public void Dispose()
	{
		foreach (var storage in storages)
		{
			storage.Key.Dispose();
		}
	}
}