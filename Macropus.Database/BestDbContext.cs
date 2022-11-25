using System.Data.Common;
using Macropus.Database.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Macropus.Database;

public abstract class BestDbContext : DbContext, IBestDbContext
{
	private DbConnection? dbConnection;

	void IBestDbContext.SetDbConnection(DbConnection connection)
	{
		dbConnection = connection;
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlite(dbConnection!);
	}

	public override void Dispose()
	{
		base.Dispose();

		dbConnection?.Dispose();
	}

	public override async ValueTask DisposeAsync()
	{
		await base.DisposeAsync();

		if (dbConnection != null)
			await dbConnection.DisposeAsync();
	}
}