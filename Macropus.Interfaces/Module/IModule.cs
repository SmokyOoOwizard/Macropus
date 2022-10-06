namespace Macropus.Interfaces.Module;

public interface IModule : IModuleBase
{
    static abstract IModuleRequiresPermissions GetRequiresPermissions();
}