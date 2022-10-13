using System.Data.Common;
using Macropus.Database.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace Macropus.Database.Db;

public class DatabasesDbContext : DbContext
{
	private readonly DbConnection dbConnection;

	public DbSet<DatabaseDbModel> Databases { get; set; }

	public DatabasesDbContext(DbConnection dbConnection)
	{
		this.dbConnection = dbConnection;
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlite(dbConnection);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new DatabaseConfig());
	}

	public override void Dispose()
	{
		base.Dispose();

		dbConnection.Dispose();
	}

	public override async ValueTask DisposeAsync()
	{
		await base.DisposeAsync();

		await dbConnection.DisposeAsync();
	}
}