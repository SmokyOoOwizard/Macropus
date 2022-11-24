using System.Collections.Concurrent;
using Autofac;
using Macropus.CoolStuff;
using Macropus.Project.Raw;

namespace Macropus.Project.Instance.Impl;

public class ProjectService : IProjectService
{
	private readonly ConcurrentDictionary<Guid, IProjectInstance> projects = new();
	private readonly KeyedLock<Guid> keyedLock = new();

	private readonly ILifetimeScope scope;
	private readonly IRawProjectService rawProjectService;

	public ProjectService(ILifetimeScope scope, IRawProjectService rawProjectService)
	{
		this.scope = scope;
		this.rawProjectService = rawProjectService;
	}

	public async Task<IProjectInstance> GetOrLoadAsync(Guid projectId, CancellationToken cancellationToken = default)
	{
		using (keyedLock.Lock(projectId))
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

			var projectProvider = await rawProjectService.GetOrLoadAsync(projectId, cancellationToken);
			var projectScope = scope.BeginLifetimeScope(b =>
			{
				b.RegisterInstance(projectProvider).AsSelf().AsImplementedInterfaces();
			});

			try
			{
				var project = projectScope.Resolve<IProjectInstance>(new NamedParameter("scope", projectScope));

				projects.TryAdd(projectId, project);
				
				if (project is IMakeRef<IProjectInstance> refProject)
				{
					project = refProject.MakeRef();
				}

				return project;
			}
			catch
			{
				projectScope.Dispose();
				throw;
			}
		}
	}
}