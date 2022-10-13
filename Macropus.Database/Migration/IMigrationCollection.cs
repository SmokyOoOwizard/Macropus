using System.Collections;

namespace Macropus.Database.Migration;

public interface IMigrationCollection : IReadOnlyCollection<IMigration>, IEnumerable
{
    uint LastVersion { get; }
}