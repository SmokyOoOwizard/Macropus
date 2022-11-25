using System.Collections;

namespace Macropus.Database.Interfaces.Migration;

public interface IMigrationCollection : IReadOnlyCollection<IMigration>, IEnumerable
{
    uint LastVersion { get; }
}