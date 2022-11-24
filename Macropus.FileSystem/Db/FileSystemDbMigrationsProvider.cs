using Macropus.Database.Migration;
using Macropus.Database.Migration.Impl;
using Macropus.FileSystem.Db.Migrations;

namespace Macropus.FileSystem.Db;

internal class FileSystemDbMigrationsProvider : IMigrationsProvider
{
	private static readonly MigrationCollection migrations = new(
		new m1_BaseFileSystemDbMigration()
	);

	public static uint LastVersion => migrations.LastVersion;

	public static IMigrationCollection GetMigrations()
	{
		return migrations;
	}
}