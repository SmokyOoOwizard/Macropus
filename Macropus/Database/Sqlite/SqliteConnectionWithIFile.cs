using Macropus.FileSystem;
using Microsoft.Data.Sqlite;

namespace Macropus.Database.Sqlite;

public class SqliteConnectionWithIFile : SqliteConnection
{
	private readonly IFileProviderInternal fileProvider;

	public SqliteConnectionWithIFile(IFileProviderInternal fileProvider) : base($"Data Source={fileProvider.Path}")
	{
		this.fileProvider = fileProvider;
	}


	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);

		if (disposing)
			fileProvider.Dispose();
	}

	public override async ValueTask DisposeAsync()
	{
		await base.DisposeAsync();

		await fileProvider.DisposeAsync();
	}
}