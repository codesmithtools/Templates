using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSmith.Data.Linq;
using NUnit.Framework;
using Tracker.Core.Data;

namespace Tracker.Tests
{
    [TestFixture]
    public class IncludeTests
    {
        [Test]
        public void IncludeFirstOrDefault()
        {
            var db = new TrackerDataContext { Log = Console.Out };
            
            var tasks = db.Task
                .Where(t => t.CreatedId != null)
                .Include(t => t.CreatedUser);

            // loads all, then does first
            var task = tasks.FirstOrDefault();


            Assert.IsNotNull(task);
            Assert.IsNotNull(task.CreatedUser);
        }

        [Test]
        public void IncludeManyToOne()
        {
            List<Task> tasks;

            var db = new TrackerDataContext {Log = Console.Out};
            tasks = db.Task
                .Take(5)
                .ToList();
            Assert.IsNotNull(tasks);

            Task task = tasks.FirstOrDefault();
            // detach to prevent exception
            task.Detach();

            Assert.IsNotNull(task);
            // should be null, context disposed
            Assert.IsNull(task.CreatedUser);


            db = new TrackerDataContext {Log = Console.Out};
            tasks = db.Task
                .Take(5)
                .Include(t => t.CreatedUser)
                .ToList();


            Assert.IsNotNull(tasks);

            task = tasks.FirstOrDefault();

            // detach to prevent exception
            //task.Detach();

            Assert.IsNotNull(task);
            // should be loaded even though context is disposed
            Assert.IsNotNull(task.CreatedUser);
        }

        [Test]
        public void IncludeOneToMany()
        {
            List<User> users;

            var db = new TrackerDataContext { Log = Console.Out };
            users = db.User
                .Take(5)
                .ToList();
            Assert.IsNotNull(users);

            User user = users.FirstOrDefault();
            // detach to prevent exception
            user.Detach();

            Assert.IsNotNull(user);
            // should be null, context disposed
            Assert.IsNotNull(user.CreatedTaskList);
            Assert.AreEqual(0, user.CreatedTaskList.Count);

            db = new TrackerDataContext { Log = Console.Out };
            users = db.User
                .Take(5)
                .Include(t => t.CreatedTaskList)
                .ToList();


            Assert.IsNotNull(users);

            user = users.FirstOrDefault();

            // detach to prevent exception
            //user.Detach();

            Assert.IsNotNull(user);
            // should be loaded even though context is disposed
            Assert.IsNotNull(user.CreatedTaskList);
            Assert.GreaterOrEqual(user.CreatedTaskList.Count, 1);
        }



    }
}
