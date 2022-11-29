namespace Macropus.Project.Instance;

public interface IProjectService
{
	Task<IProjectInstance> GetOrLoadAsync(Guid projectId, CancellationToken cancellationToken = default);
}