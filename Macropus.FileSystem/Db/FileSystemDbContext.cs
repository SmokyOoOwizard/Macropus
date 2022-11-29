using System.Data.Common;
using Macropus.Database;
using Macropus.FileSystem.Db.Models.File;
using Microsoft.EntityFrameworkCore;

namespace Macropus.FileSystem.Db;

internal class FileSystemDbContext : BestDbContext
{
	public DbSet<FileDbModel> Files { get; set; }


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new FileConfig());
	}
}