using System;
using System.Data.Linq;
using System.Linq;

namespace Tracker.Data
{
    public static partial class TaskExtensions
    {
        //Add query extension methods here.
        
        #region Query
        // A private class for lazy loading static compiled queries.
        private static partial class Query
        {
            // Add your compiled queries here. 
        } 
        #endregion
    }
}