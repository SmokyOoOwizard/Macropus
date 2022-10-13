namespace Macropus.Database.Migration;

public interface IMigrationsProvider
{
    static abstract uint LastVersion { get; }
    static abstract IMigrationCollection GetMigrations();
}