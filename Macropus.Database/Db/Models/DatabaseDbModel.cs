using System.ComponentModel.DataAnnotations.Schema;

namespace Macropus.Database.Db.Models
{
	[Table("Databases")]
	internal class DatabaseDbModel
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public Guid FileId { get; set; }
	}
}