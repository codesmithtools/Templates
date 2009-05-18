using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tester.Data;

namespace Testert.Tests
{
    [TestFixture]
    public class QueryTests
    {

        [Test]
        public void Example()
        {
            TesterDataContext db = new TesterDataContext();
            db.AuditingEnabled = true;

            UserAccount ua = new UserAccount();
            ua.UserAccountID = Guid.NewGuid();
            ua.FirstName = "Troy";
            ua.LastName = "Zarger";
            ua.ZipCode = "30040";
            db.UserAccount.InsertOnSubmit(ua);
            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }


        }
    }
}
