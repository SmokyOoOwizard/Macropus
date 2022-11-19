using Macropus.FileSystem;

namespace Macropus.Module;

public interface IModulesProvider : IDisposable
{
    Task<IModuleInfo[]> GetModulesInfoAsync(CancellationToken cancellationToken = default);
    Task<IFileProvider> GetModuleAsync(IModuleInfo moduleInfo, CancellationToken cancellationToken = default);
}