using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using NHibernate;
using Sakila.Data.Entities;

namespace Sakila.Data
{
    public partial class SakilaDataContext
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