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

      var users = DynamicQueryable.Select(db.Users, "new(Id, Email)");

      var q = users as ObjectQuery;
      string sql = q.ToTraceString();

      db.Users.Delete(u => u.Email.EndsWith("@test.com"));
    }
  }
}
