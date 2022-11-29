using Macropus.Interfaces.User;

namespace Macropus.Project.Storage.Impl;

public readonly struct ProjectCreationInfo : IProjectCreationInfo
{
    public string Name { get; init; }
    public IUser User { get; init; }
}