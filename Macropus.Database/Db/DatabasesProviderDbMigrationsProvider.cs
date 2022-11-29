using Macropus.Database.Db.Migrations;
using Macropus.Database.Interfaces.Migration;
using Macropus.Database.Migration;

namespace Macropus.Database.Db;

internal class DatabasesProviderDbMigrationsProvider : IMigrationsProvider
{
	private static readonly MigrationCollection migrations = new(
		new m1_BaseDatabasesProviderDbMigration()
	);

	public static uint LastVersion => migrations.LastVersion;

	public static IMigrationCollection GetMigrations()
	{
		return migrations;
	}
}