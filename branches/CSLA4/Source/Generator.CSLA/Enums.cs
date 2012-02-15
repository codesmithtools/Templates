using System;

namespace Generator.CSLA
{
    #region Enumeration(s)

    public enum DataAccessMethod : byte 
    {
        None = 0,
        ParameterizedSQL = 1,
        StoredProcedures = 2, 
        LinqToSQL = 3,
        ObjectFactoryNone = 10,
        ObjectFactoryParameterizedSQL = 11,
        ObjectFactoryStoredProcedures = 12
    }

    public enum ProjectTypeEnum
    {
        None = 0,
        DynamicDataWebApp = 1,
        DynamicDataWebSite = 2
    }

    public enum TransactionIsolationLevelEnum
    {
        ReadCommitted,
        ReadUncommitted,
        RepeatableRead,
        Serializable
    }

    #endregion
}
