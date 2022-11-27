using Autofac;
using Macropus.Interfaces.User;
using Macropus.Project.Instance;

namespace Macropus.Project.Connection.Impl;

internal class ConnectionService : IConnectionService
{
	private readonly ILifetimeScope scope;
	private readonly IProjectService projectService;

	public ConnectionService(ILifetimeScope scope, IProjectService projectService)
	{
		this.scope = scope;
		this.projectService = projectService;
	}

	public async Task<IProjectConnection> Connect(IUser user, Guid projectId)
	{
		var project = await projectService.GetOrLoadAsync(projectId).ConfigureAwait(false);

		return scope.Resolve<ProjectConnection>(new TypedParameter(typeof(IProjectInstance), project));
	}
}