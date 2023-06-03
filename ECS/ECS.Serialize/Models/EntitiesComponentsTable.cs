using LinqToDB.Mapping;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
#pragma warning disable CS8618

namespace ECS.Serialize.Models;

[Table(TABLE_NAME)]
public class EntitiesComponentsTable
{
	public const string TABLE_NAME = "EntitiesComponents";

	[PrimaryKey, Identity]
	public int Id { get; set; }

	[Column, NotNull]
	public int ComponentId { get; set; }

	[Column, NotNull]
	public string ComponentName { get; set; }

	[Column, NotNull]
	public string EntityId { get; set; }
}