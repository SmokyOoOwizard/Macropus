using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Macropus.Module.Db;

internal class ModulesDbContext : DbContext
{
    private readonly DbConnection dbConnection;

    public ModulesDbContext(DbConnection dbConnection)
    {
        this.dbConnection = dbConnection;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(dbConnection);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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