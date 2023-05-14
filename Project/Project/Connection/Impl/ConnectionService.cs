using Autofac;
using Delogger.Scope;
using Delogger.Scope.Log;
using Macropus.Project.Instance;

namespace Macropus.Project.Connection.Impl;

internal class ConnectionService : IConnectionService
{
	private readonly ILifetimeScope scope;
	private readonly IProjectService projectService;
	private readonly IDScope dScope;
	private readonly IDLogger logger;

	public ConnectionService(ILifetimeScope scope, IProjectService projectService)
	{
		this.scope = scope;
		this.projectService = projectService;

		dScope = scope.Resolve<IDScope>();
		logger = dScope.CreateLogger(new LoggerCreateOptions() { Tags = new[] { nameof(ConnectionService) } });
	}

	public async Task<IProjectConnection> Connect(Guid projectId)
	{
		logger.Log("New project connection",
			new[] { "Connect" }, null,
			new KeyValuePair<string, object>[]
			{
				new("Project Id", projectId),
			});

		var project = await projectService.GetOrLoadAsync(projectId).ConfigureAwait(false);

		return scope.Resolve<ProjectConnection>(new TypedParameter(typeof(IProjectInstance), project));
	}
}