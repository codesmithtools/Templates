//------------------------------------------------------------------------------
//
// Copyright (c) 2002-2012 CodeSmith Tools, LLC.  All rights reserved.
// 
// The terms of use for this software are contained in the file
// named sourcelicense.txt, which can be found in the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by the
// terms of this license.
// 
// You must not remove this notice, or any other, from this software.
//
//------------------------------------------------------------------------------

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
