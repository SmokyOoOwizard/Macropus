using System.ComponentModel.DataAnnotations.Schema;

namespace Macropus.FileSystem.Db.Models.File;

[Table("Files")]
public class FileDbModel
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string ObjectName { get; set; }
}