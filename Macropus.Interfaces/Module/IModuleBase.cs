namespace Macropus.Interfaces.Module;

public interface IModuleBase
{
    Task StartAsync(IModuleAllowedPermissions allowedPermissions, CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);

    void GetModuleContainer(IContainerBuilder builder);
}