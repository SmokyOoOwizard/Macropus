using Macropus.Database.Interfaces.Migration;

namespace Macropus.Database.Interfaces;

public interface IDbContextService
{
	Task<T> GetOrCreateDbContextAsync<T, TM>(
		string path,
		CancellationToken cancellationToken = default
	) where T : IBestDbContext, new() where TM : IMigrationsProvider;
}