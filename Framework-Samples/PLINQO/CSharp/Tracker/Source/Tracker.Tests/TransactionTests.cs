using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using NUnit.Framework;
using CodeSmith.Data.Linq;
using Tracker.Core.Data;

namespace Tracker.Tests
{
    [TestFixture]
    public class TransactionTests
    {
        [SetUp]
        public void Setup()
        {
            //TODO: NUnit setup
        }

        [TearDown]
        public void TearDown()
        {
            //TODO: NUnit TearDown
        }

        [Test]
        public void TransactionScope()
        {
            List<User> users;
            List<Task> tasks;

            using (var db = new TrackerDataContext { Log = Console.Out })
            using (var t = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                users = db.User.ToList();
                tasks = db.Task.ToList();
            }

            Assert.IsNotNull(users);
            Assert.IsNotNull(tasks);
        }

        [Test]
        public void BeginTransaction()
        {
            List<User> users;
            List<Task> tasks;

            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                db.Connection.Open();
                using (db.Transaction = db.Connection.BeginTransaction(
                    System.Data.IsolationLevel.ReadUncommitted))
                {
                    users = db.User.ToList();
                    tasks = db.Task.ToList();
                }
            }

            Assert.IsNotNull(users);
            Assert.IsNotNull(tasks);
        }

        [Test]
        public void ExecuteCommand()
        {
            List<User> users;
            List<Task> tasks;

            using (var db = new TrackerDataContext { Log = Console.Out })
            {
                db.Connection.Open();
                db.ExecuteCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");

                users = db.User.ToList();
                tasks = db.Task.ToList();
            }

            Assert.IsNotNull(users);
            Assert.IsNotNull(tasks);
        }
    }
}
