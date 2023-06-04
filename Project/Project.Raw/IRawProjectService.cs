namespace Macropus.Project.Raw.Raw;

public interface IRawProjectService
{
	Task<IRawProject> GetOrLoadAsync(string path, CancellationToken cancellationToken = default);
}