using System;
using System.Collections.Generic;
using System.Text;

namespace LinqToSqlShared.Generator
{
    public enum FrameworkEnum
    {
        v35 = 1,
        v35_SP1 = 2
    }

    public enum TableNamingEnum
    {
        Plural = 1,
        Singluar = 2,
        Mixed = 3
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
