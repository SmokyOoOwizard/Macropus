using YamlDotNet.Serialization;

namespace Macropus.Project.Impl;

internal class ProjectInformationInternal : IProjectInformationInternal
{
	public Guid Id { get; set; }
	public string Name { get; set; }

	[YamlIgnore]
	public string Path { get; set; }
}