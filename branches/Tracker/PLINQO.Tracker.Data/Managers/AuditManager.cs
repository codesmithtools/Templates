
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using CodeSmith.Data.Rules;
using CodeSmith.Data.Rules.Validation;

namespace PLINQO.Tracker.Data
{
    public partial class AuditManager
    {
        #region Extensibility Method Definitions
        //static partial void AddRules()
        //{
        //    // Add shared rules here
        //}
        #endregion
        
        #region Query
        // A private class for lazy loading static compiled queries.
        private static partial class Query
        {
            // Add your compiled queries here. 
        } 
        #endregion
    }
}