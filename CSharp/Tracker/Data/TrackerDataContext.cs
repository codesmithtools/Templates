using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using NHibernate;
using Tracker.Data.Entities;

namespace Tracker.Data
{
    public partial class TrackerDataContext
    {
        // Place your custom code here.
        
        #region Override Methods

        protected override string GetConnectionString(string databaseName)
        {
            return base.GetConnectionString(databaseName);
        }

        #endregion
    }
}