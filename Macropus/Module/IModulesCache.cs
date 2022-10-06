using Macropus.FileSystem;
using Macropus.Stuff.Cache;

namespace Macropus.Module;

internal interface IModulesCache : IDisposable
{
    Task<ICacheRef<IModuleContainer, string>> GetOrLoadModuleAsync(IFileProvider file,
        CancellationToken cancellationToken);
}