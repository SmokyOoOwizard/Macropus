using Macropus.Database.Db.Migrations;
using Macropus.Database.Migration;
using Macropus.Database.Migration.Impl;

namespace Macropus.DatabasesProvider.Db;

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