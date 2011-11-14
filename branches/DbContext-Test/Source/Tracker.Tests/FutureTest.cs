using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using EntityFramework.Extensions;
using EntityFramework.Future;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tracker.Entity;

namespace Tracker.Tests
{
  [TestClass]
  public class FutureTest
  {
    [TestMethod]
    public void TestMethod1()
    {
      var db = new TrackerEntities();

      // build up queries

      var q1 = db.Users
        .Where(p => p.Email == "one@test.com");


      var command = db.GetCommand(q1);
      Assert.IsNotNull(command);
    }

    [TestMethod]
    public void PageTest()
    {
      var db = new TrackerEntities();

      // base query
      var q = db.Tasks
          .Where(p => p.PriorityId == 2)
          .OrderByDescending(t => t.CreatedDate);

      // get total count
      var q1 = q.FutureCount();
      // get first page
      var q2 = q.Skip(0).Take(10).Future();
      // triggers sql execute as a batch
      var tasks = q2.ToList();
      int total = q1.Value;

      Assert.IsNotNull(tasks);
    }

    [TestMethod]
    public void SimpleTest()
    {
      var db = new TrackerEntities();

      // build up queries

      string emailDomain = "@battlestar.com";
      var q1 = db.Users
          .Where(p => p.Email.EndsWith(emailDomain))
          .Future();

      string search = "Earth";
      var q2 = db.Tasks
          .Where(t => t.Summary.Contains(search))
          .Future();

      // should be 2 queries 
      //Assert.AreEqual(2, db.FutureQueries.Count);

      // this triggers the loading of all the future queries
      var users = q1.ToList();
      Assert.IsNotNull(users);

      // should be cleared at this point
      //Assert.AreEqual(0, db.FutureQueries.Count);

      // this should already be loaded
      Assert.IsTrue(((IFutureQuery)q2).IsLoaded);

      var tasks = q2.ToList();
      Assert.IsNotNull(tasks);

    }

    [TestMethod]
    public void FutureCountTest()
    {
      var db = new TrackerEntities();

      // build up queries

      var q1 = db.Users
          .Where(p => p.Email == "one@test.com")
          .Future();

      var q2 = db.Tasks
          .Where(t => t.Summary == "Test")
          .FutureCount();

      // should be 2 queries 
      //Assert.AreEqual(2, db.FutureQueries.Count);

      // this triggers the loading of all the future queries
      var users = q1.ToList();
      Assert.IsNotNull(users);

      // should be cleared at this point
      //Assert.AreEqual(0, db.FutureQueries.Count);

      // this should already be loaded
      Assert.IsTrue(((IFutureQuery)q2).IsLoaded);

      int count = q2;
      Assert.AreNotEqual(count, 0);
    }

    [TestMethod]
    public void FutureCountReverseTest()
    {
      var db = new TrackerEntities();

      // build up queries

      var q1 = db.Users
          .Where(p => p.Email == "one@test.com")
          .Future();

      var q2 = db.Tasks
          .Where(t => t.Summary == "Test")
          .FutureCount();

      // should be 2 queries 
      //Assert.AreEqual(2, db.FutureQueries.Count);

      // access q2 first to trigger loading, testing loading from FutureCount
      // this triggers the loading of all the future queries
      var count = q2.Value;
      Assert.AreNotEqual(count, 0);

      // should be cleared at this point
      //Assert.AreEqual(0, db.FutureQueries.Count);

      // this should already be loaded
      Assert.IsTrue(((IFutureQuery)q1).IsLoaded);

      var users = q1.ToList();
      Assert.IsNotNull(users);
    }

    [TestMethod]
    public void FutureValueTest()
    {
      var db = new TrackerEntities();

      // build up queries
      var q1 = db.Users
          .Where(p => p.Email == "one@test.com")
          .FutureValue();

      var q2 = db.Tasks
          .Where(t => t.Summary == "Test")
          .FutureCount();

      // duplicate query except count
      var q3 = db.Tasks
          .Where(t => t.Summary == "Test")
          .Future();

      // should be 3 queries 
      //Assert.AreEqual(3, db.FutureQueries.Count);

      // this triggers the loading of all the future queries
      User user = q1;
      Assert.IsNotNull(user);

      // should be cleared at this point
      //Assert.AreEqual(0, db.FutureQueries.Count);

      // this should already be loaded
      Assert.IsTrue(((IFutureQuery)q2).IsLoaded);

      var count = q2.Value;
      Assert.AreNotEqual(count, 0);

      var tasks = q3.ToList();
      Assert.IsNotNull(tasks);
    }

    [TestMethod]
    public void FutureValueReverseTest()
    {
      var db = new TrackerEntities();
      // build up queries

      var q1 = db.Users
          .Where(u => u.Email == "one@test.com")
          .FutureValue();

      var q2 = db.Tasks
          .Where(t => t.Summary == "Test")
          .FutureCount();

      // duplicate query except count
      var q3 = db.Tasks
          .Where(t => t.Summary == "Test")
          .Future();

      // should be 3 queries 
      //Assert.AreEqual(3, db.FutureQueries.Count);

      // access q2 first to trigger loading, testing loading from FutureCount
      // this triggers the loading of all the future queries
      var count = q2.Value;
      Assert.AreNotEqual(count, 0);

      // should be cleared at this point
      //Assert.AreEqual(0, db.FutureQueries.Count);

      // this should already be loaded
      Assert.IsTrue(((IFutureQuery)q1).IsLoaded);

      var users = q1.Value;
      Assert.IsNotNull(users);

      var tasks = q3.ToList();
      Assert.IsNotNull(tasks);

    }

  }
}
