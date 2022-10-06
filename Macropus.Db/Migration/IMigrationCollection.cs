using System.Collections;

namespace Macropus.Db.Migration;

public interface IMigrationCollection : IReadOnlyCollection<IMigration>, IEnumerable
{
    uint LastVersion { get; }
}