using System;

namespace LinqToSqlShared.Generator
{
    public enum FrameworkEnum
    {
        v35 = 1,
        v35_SP1 = 2,
        v40 = 3,
        v45 = 4
    }

    public enum TableNamingEnum
    {
        Mixed = 0,
        Plural = 1,
        Singular = 2
    }

    public enum EntityNamingEnum
    {
        Preserve = 0,
        Plural = 1,
        Singular = 2
    }

    public enum AssociationNamingEnum
    {
        None = 0,
        Plural = 1,
        ListSuffix = 2
    }
}
