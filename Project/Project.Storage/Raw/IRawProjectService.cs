using Macropus.Project.Storage.Raw;

namespace Macropus.Project.Raw;

public interface IRawProjectService
{
	Task<IRawProject> GetOrLoadAsync(Guid projectId, CancellationToken cancellationToken = default);
}