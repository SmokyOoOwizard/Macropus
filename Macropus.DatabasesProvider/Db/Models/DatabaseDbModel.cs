using System.ComponentModel.DataAnnotations.Schema;

namespace Macropus.DatabasesProvider.Db.Models;

[Table("Databases")]
public class DatabaseDbModel
{
	public Guid Id { get; set; }

	public string Name { get; set; }

	public Guid FileId { get; set; }
}