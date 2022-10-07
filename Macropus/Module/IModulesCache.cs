using Macropus.FileSystem;
using Macropus.Stuff.Cache;

namespace Macropus.Module;

internal interface IModulesCache : IDisposable
{
	Task<ICacheRef<IRawModuleContainer, string>> GetOrLoadModuleAsync(IFileProvider file,
		CancellationToken cancellationToken);
}