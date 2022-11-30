using System.Collections.Concurrent;
using Autofac;
using Macropus.CoolStuff;
using Macropus.Project.Storage.Impl;

namespace Macropus.Project.Raw.Impl;

public class RawProjectService : IRawProjectService
{
	private readonly ConcurrentDictionary<Guid, IRawProject> projects = new();
	private readonly KeyedLock<Guid> keyedLock = new();

	private readonly ILifetimeScope scope;
	private readonly ProjectsStorageMaster storageMaster;

	public RawProjectService(ILifetimeScope scope, ProjectsStorageMaster storageMaster)
	{
		this.scope = scope;
		this.storageMaster = storageMaster;
	}

	public async Task<IRawProject> GetOrLoadAsync(Guid projectId, CancellationToken cancellationToken = default)
	{
		using (keyedLock.Lock(projectId))
		{
			IRawProject project;

			if (projects.ContainsKey(projectId))
				project = projects[projectId];
			else
			{
				project = await storageMaster.OpenProjectAsync(projectId, cancellationToken);
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