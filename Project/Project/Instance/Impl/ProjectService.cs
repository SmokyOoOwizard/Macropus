using System.Collections.Concurrent;
using AsyncKeyedLock;
using Autofac;
using Macropus.CoolStuff;
using Macropus.Project.Storage.Impl;

namespace Macropus.Project.Instance.Impl;

public class ProjectService : IProjectService, IDisposable
{
	private readonly ConcurrentDictionary<Guid, IProjectInstance> projects = new();
	private readonly AsyncKeyedLocker<Guid> keyedLock = new();

	private readonly ILifetimeScope scope;
	private readonly ProjectsStorageMaster projectsStorage;

	public ProjectService(ILifetimeScope scope, ProjectsStorageMaster projectsStorage)
	{
		this.scope = scope;
		this.projectsStorage = projectsStorage;
	}

	public async Task<IProjectInstance> GetOrLoadAsync(Guid projectId, CancellationToken cancellationToken = default)
	{
		using (await keyedLock.LockAsync(projectId, cancellationToken).ConfigureAwait(false))
		{
			if (projects.ContainsKey(projectId))
			{
				var project = projects[projectId];

				if (project is IMakeRef<IProjectInstance> refProject)
				{
					project = refProject.MakeRef();
				}

				return project;
			}

			return await LoadAsync(projectId, cancellationToken);
		}
	}

	private async Task<IProjectInstance> LoadAsync(Guid projectId, CancellationToken cancellationToken)
	{
		var projectProvider = await projectsStorage.OpenProjectAsync(projectId, cancellationToken);
		var projectScope = scope.BeginLifetimeScope(b =>
		{
			b.RegisterInstance(projectProvider).AsSelf().AsImplementedInterfaces();
		});

		try
		{
			var project = projectScope.Resolve<IProjectInstance>(new NamedParameter("scope", projectScope));

			projects.TryAdd(projectId, project);

			await project.InitializeAsync(cancellationToken);
			
			if (project is IMakeRef<IProjectInstance> refProject)
			{
				return refProject.MakeRef();
			}

			return project;
		}
		catch
		{
			projectScope.Dispose();
			throw;
		}
	}

	public void Dispose()
	{
		scope.Dispose();
	}
}