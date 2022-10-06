namespace Macropus.FileSystem.Impl;

internal class FileProvider : IFileProvider
{
    private readonly string path;

    private readonly FileStream fileStream;

    public string? Name { get; }

    public Guid Id { get; }

    public FileMode Mode { get; }

    public FileAccess Access { get; }

    public FileShare Share { get; }

    public Stream AsStream => fileStream;

    public FileProvider(string path, string? name, Guid id, FileMode fileMode, FileAccess fileAccess,
        FileShare fileShare, FileStream fileStream)
    {
        this.path = path;
        Name = name;
        Id = id;
        Mode = fileMode;
        Access = fileAccess;
        Share = fileShare;
        this.fileStream = fileStream;
    }

    public void Dispose()
    {
        fileStream.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await fileStream.DisposeAsync();
    }

    public static FileProvider Create(string path, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
    {
        return Create(path, string.Empty, Guid.Empty, fileMode, fileAccess, fileShare);
    }

    public static FileProvider Create(string path, string? name, Guid id, FileMode fileMode, FileAccess fileAccess,
        FileShare fileShare)
    {
        if (path == null) throw new ArgumentNullException(nameof(path));

        if (name == null)
            name = string.Empty;

        var fileStream = new FileStream(path, fileMode, fileAccess, fileShare);

        return new FileProvider(path, name, id, fileMode, fileAccess, fileShare, fileStream);
    }
}