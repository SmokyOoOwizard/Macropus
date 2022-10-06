using System.Collections;

namespace Macropus.Db.Migration.Impl;

public sealed class MigrationCollection : IMigrationCollection
{
    public uint LastVersion { get; }

    public int Count => migrations.Length;

    private readonly IMigration[] migrations;

    public MigrationCollection(params IMigration[] migrations)
    {
        if (migrations == null || migrations.Length == 0) throw new ArgumentNullException(nameof(migrations));

        if (migrations.Length > 1)
        {
            var diff = migrations[0].Version - (int)migrations[1].Version;
            if (diff < 0)
            {
                for (var i = 0; i < migrations.Length - 1; i++)
                    if (migrations[i].Version + 1 != migrations[i + 1].Version)
                        // TODO throw migrations versions must be sequence
                        throw new Exception();
            }
            else if (diff > 0)
            {
                for (var i = 0; i < migrations.Length - 1; i++)
                    if (migrations[i].Version - 1 != migrations[i + 1].Version)
                        // TODO throw migrations versions must be sequence
                        throw new Exception();

                migrations = migrations.Reverse().ToArray();
            }
            else
            {
                // TODO throw two or more migrations with same versions
                throw new Exception();
            }
        }

        LastVersion = migrations.Last().Version;

        this.migrations = migrations;
    }

    public IEnumerator<IMigration> GetEnumerator()
    {
        return ((IEnumerable<IMigration>)migrations).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return migrations.GetEnumerator();
    }
}