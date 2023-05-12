using System.Data.Common;

namespace Macropus.Database.Interfaces;

public interface IBestDbContext
{
	void SetDbConnection(DbConnection connection);
}