using Macropus.Interfaces.User;

namespace Macropus.Project.Storage;

public interface IProjectCreationInfo
{
    string Name { get; }
    IUser User { get; }
}