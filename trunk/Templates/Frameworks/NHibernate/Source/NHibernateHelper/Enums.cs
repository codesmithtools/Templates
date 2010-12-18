using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernateHelper
{
    public enum NHibernateVersion
    {
        v1_2,
        v2_1
    }

    public enum VisualStudioVersion
    {
        VS_2005,
        VS_2008,
        VS_2010
    }

    public enum MethodNameGenerationMode
    {
        Default,
        ExtendedProperty,
        Custom
    }

    public enum AssociationTypeEnum
    {
        ManyToOne,
        OneToMany,
        ManyToMany
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
        Table = 0,
        Column = 1
    }

    public enum AssociationSuffixEnum
    {
        None = 0,
        Plural = 1,
        List = 2
    }

    public enum SchemaProvider
    {
        ADOXSchemaProvider,
        MySQLSchemaProvider,
        OracleSchemaProvider,
        PostgreSchemaProvider,
        PostgreSQLSchemaProvider,
        SqlCompactSchemaProvider,
        SQLiteSchemaProvider,
        SqlSchemaProvider,
        VistaDBSchemaProvider
    }
}
