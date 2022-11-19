using System.Collections.Concurrent;
using Macropus.Interfaces.User;
using Macropus.Linq;
using Macropus.Project.Connection;

namespace Macropus.Project.Storage.Impl;

public class ProjectsStorageMaster : IProjectsStorage
{
	private readonly ConcurrentDictionary<IProjectsStorage, bool> storages = new();
	private IProjectsStorage? defaultStorage;

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

	public async Task<IProjectConnection> OpenProjectAsync(
		Guid id,
		IUser user,
		CancellationToken cancellationToken = default
	)
	{
		foreach (var storage in storages)
		{
			try
			{
				return await storage.Key.OpenProjectAsync(id, user, cancellationToken).ConfigureAwait(false);
			}
			catch
			{
				// ignored
			}
		}

		throw new NotImplementedException();
	}

	public async Task<bool> DeleteProjectAsync(Guid id, IUser user, CancellationToken cancellationToken = default)
	{
		foreach (var storage in storages)
		{
			if (await storage.Key.DeleteProjectAsync(id, user, cancellationToken).ConfigureAwait(false))
				return true;
		}

		return false;
	}

	public Task GetProjectInformation(Guid projectId, IUser user, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public async Task<Guid[]> GetExistsProjectsAsync(IUser user, CancellationToken cancellationToken = default)
	{
		var ids = new HashSet<Guid>();
		foreach (var storage in storages)
		{
			ids.AddRange(await storage.Key.GetExistsProjectsAsync(user, cancellationToken).ConfigureAwait(false));
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