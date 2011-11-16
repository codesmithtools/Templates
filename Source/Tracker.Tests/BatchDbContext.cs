using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tracker.Core;
using Tracker.Core.Entities;

namespace Tracker.Tests
{
    [TestClass]
    public class BatchDbContext
    {
        [TestMethod]
        public void Delete()
        {
            var db = new TrackerContext();
            string emailDomain = "@test.com";
            int count = db.User.Delete(u => u.EmailAddress.EndsWith(emailDomain));
        }

        [TestMethod]
        public void Update()
        {
            var db = new TrackerContext();
            string emailDomain = "@test.com";
            int count = db.User.Update(
                u => u.EmailAddress.EndsWith(emailDomain),
                u => new User { IsApproved = false, LastActivityDate = DateTime.Now });
        }

    }
}
