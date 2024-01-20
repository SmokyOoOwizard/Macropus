using System.Collections.Concurrent;
using AsyncKeyedLock;
using Macropus.CoolStuff;
using Macropus.Project.Raw.Raw;

namespace Macropus.Project.Raw.Impl;

public class RawProjectService : IRawProjectService
{
	private readonly ConcurrentDictionary<Guid, IRawProject> projects = new();
	private readonly AsyncKeyedLocker<Guid> keyedLock = new(o =>
	{
		o.PoolSize = 20;
		o.PoolInitialFill = 1;
	});

	private readonly RawProjectFactory rawProjectFactory;

	public RawProjectService(RawProjectFactory rawProjectFactory)
	{
		this.rawProjectFactory = rawProjectFactory;
	}

	public async Task<IRawProject> GetOrLoadAsync(string path, CancellationToken cancellationToken = default)
	{
		var projectInfo = await RawProjectFactory.TryGetProjectInfo(path, cancellationToken);
		if (projectInfo == null)
			throw new(); // TODO

		var projectId = projectInfo.Id;

		using (await keyedLock.LockAsync(projectId, cancellationToken).ConfigureAwait(false))
		{
			IRawProject project;

			if (projects.ContainsKey(projectId))
				project = projects[projectId];
			else
			{
				project = await rawProjectFactory.Open(path, cancellationToken);
				projects.TryAdd(projectId, project);
			}

			if (project is IMakeRef<IRawProject> refProject)
			{
				project = refProject.MakeRef();
			}

			return project;
		}
	}
}