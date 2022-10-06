namespace Macropus.Project.Storage;

internal static class ProjectsStorageLocalUtils
{
    public static async Task<string> FindProjectAsync(string directory, Guid id,
        CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(directory))
            // TODO throw directory not found
            throw new Exception();

        var subDirectories = Directory.GetDirectories(directory);
        foreach (var subDirectory in subDirectories)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var projInfo = await ProjectUtils.TryGetProjectInfo(subDirectory, cancellationToken);
            if (projInfo == null)
                continue;


            if (projInfo.Id == id) return subDirectory;
        }

        // TODO throw project not find
        throw new Exception();
    }
}