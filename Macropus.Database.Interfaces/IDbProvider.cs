using System.Data.Common;

namespace Macropus.Database.Interfaces;

public interface IDbProvider
{
    Guid Id { get; }
    string Name { get; }

    uint Version { get; }

    DbConnection CreateConnection();
}