using System.Data.Common;
using Macropus.FileSystem.Db.Models.File;
using Microsoft.EntityFrameworkCore;

namespace Macropus.FileSystem.Db;

internal class FileSystemDbContext : DbContext
{
    private readonly DbConnection dbConnection;

    public DbSet<FileDbModel> Files { get; set; }

    public FileSystemDbContext(DbConnection dbConnection)
    {
        this.dbConnection = dbConnection;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(dbConnection);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new FileConfig());
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