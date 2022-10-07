namespace Macropus.Interfaces.Module;

public interface IModuleBase : IInitializable<IModuleAllowedPermissions>, IDisposable
{
	void BindModule(IModuleBuilder builder);
}