using System;

namespace QuickStart
{
    #region Enumeration(s)

    public enum DataAccessMethod : byte 
    {
        None = 0,
        ParameterizedSQL = 1
    }

    public enum ProjectTypeEnum : byte
    {
        None = 0,
        DynamicDataWebApp = 1,
        DynamicDataWebSite = 2
    }

    #endregion
}
