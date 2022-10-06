using Macropus.FileSystem;

namespace Macropus.Module;

internal interface IModulesProvider : IDisposable
{
    Task<IModuleInfo[]> GetModulesInfoAsync(CancellationToken cancellationToken = default);
    Task<IFileProvider> GetModuleAsync(IModuleInfo moduleInfo, CancellationToken cancellationToken = default);
}