using System;
using System.ComponentModel;

namespace CodeSmith.SchemaHelper
{
    #region Enumeration(s)

    public enum LanguageEnum : byte
    {
        [Description("C#")]
        CSharp = 0,
        [Description("VisualBasic")]
        VB = 1
    }

    public enum VisualStudioVersion : byte
    {
        VS_2005 = 0,
        VS_2008 = 1,
        VS_2010 = 2
    }

    public enum Framework : byte
    {
        /// <summary>
        /// .NET 3.5
        /// </summary>
        v35 = 0,

        /// <summary>
        /// .NET 4.0
        /// </summary>
        v40 = 1
    }

    public enum AssociationType : byte
    {
        ManyToOne = 0,
        OneToMany = 1,
        ManyToMany = 2,
        OneToZeroOrOne = 3
    }

    public enum TableNaming : byte
    {
        Mixed = 0,
        Plural = 1,
        Singular = 2
    }

    public enum EntityNaming : byte
    {
        Preserve = 0,
        Plural = 1,
        Singular = 2
    }

    public enum AssociationNaming : byte
    {
        Table = 0,
        Column = 1
    }

    public enum AssociationSuffix : byte
    {
        None = 0,
        Plural = 1,
        List = 2
    }

    public enum SearchCriteriaEnum : byte
    {
        All = 0,
        PrimaryKey,
        ForeignKey,
        Index,
        NoForeignKeys
    }

    #endregion
}