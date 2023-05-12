namespace Macropus.Database.Interfaces.Migration;

public interface IMigrationsProvider
{
    static abstract uint LastVersion { get; }
    static abstract IMigrationCollection GetMigrations();
}