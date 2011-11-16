using System;
using System.Data.Objects;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EntityFramework.Extensions;
using Tracker.Entity;
using System.Linq.Dynamic;

namespace Tracker.Tests
{
    [TestClass]
    public class BatchTest
    {
        [TestMethod]
        public void Delete()
        {
            var db = new TrackerEntities();
            string emailDomain = "@test.com";
            int count = db.Users.Delete(u => u.Email.EndsWith(emailDomain));
        }

        [TestMethod]
        public void Update()
        {
            var db = new TrackerEntities();
            string emailDomain = "@test.com";
            int count = db.Users.Update(
                u => u.Email.EndsWith(emailDomain), 
                u => new User { IsApproved = false, LastActivityDate = DateTime.Now });
        }
    }
}
