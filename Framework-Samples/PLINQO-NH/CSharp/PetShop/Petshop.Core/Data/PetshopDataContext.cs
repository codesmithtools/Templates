using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using NHibernate;
using Petshop.Data.Entities;

namespace Petshop.Data
{
    public partial class PetshopDataContext
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