using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EntityFramework.Extensions;
using Tracker.Entity;

namespace Tracker.Tests
{
  [TestClass]
  public class BatchTest
  {
    [TestMethod]
    public void Delete()
    {
      var db = new TrackerEntities();
      db.Users.Delete(u => u.Email.EndsWith("@test.com"));
    }
  }
}
