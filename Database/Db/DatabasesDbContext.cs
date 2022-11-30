using Macropus.Database.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace Macropus.Database.Db;

internal class DatabasesDbContext : BestDbContext
{
	public DbSet<DatabaseDbModel> Databases { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new DatabaseConfig());
	}
}