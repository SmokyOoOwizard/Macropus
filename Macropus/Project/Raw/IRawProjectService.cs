namespace Macropus.Project.Raw;

public interface IRawProjectService
{
	Task<IRawProject> GetOrLoadAsync(Guid projectId, CancellationToken cancellationToken = default);
}