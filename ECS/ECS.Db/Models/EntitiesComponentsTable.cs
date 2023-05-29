using System.ComponentModel.DataAnnotations.Schema;

namespace ECS.Models;

[Table("EntitiesComponents")]
internal class EntitiesComponentsTable
{
	public int Id { get; set; }

	public int ComponentId { get; set; }
	public string ComponentName { get; set; }
	public string EntityId { get; set; }
}