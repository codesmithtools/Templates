using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NHibCsSample.Generated.ManagerObjects;
using NHibCsSample.Generated.BusinessObjects;

namespace NHibCsSample.Generated.Base
{
    public class UNuitTestBase
    {
        protected IManagerFactory managerFactory = new ManagerFactory();

        [SetUp]
        public void SetUp()
        {
            NHibernateSessionManager.Instance.Session.BeginTransaction();
        }
        [TearDown]
        public void TearDown()
        {
            NHibernateSessionManager.Instance.Session.RollbackTransaction();
        }
    }
}