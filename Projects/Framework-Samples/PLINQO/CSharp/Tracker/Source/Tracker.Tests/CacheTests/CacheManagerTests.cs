using System;
using System.Diagnostics;
using System.Text;
using CodeSmith.Data.Caching;
using CodeSmith.Data.Linq;

using NUnit.Framework;
using Tracker.Core.Data;

namespace Tracker.Tests.CacheTests
{
    [TestFixture]
    public class CacheManagerTests
    {
        [Test]
        public void NumberTest()
        {
            CacheManager.Set("number", 12);
            int i = CacheManager.Get<int>("number");
            Assert.AreEqual(12, i);
        }

        [Test]
        public void InvalidateTest()
        {
            CacheManager.Set("number", 12);
            int i = CacheManager.Get<int>("number");
            Assert.AreEqual(12, i);

            CacheManager.InvalidateGroup();
            i = CacheManager.Get<int>("number");
            Assert.AreEqual(0, i);
        }

        [Test]
        public void GroupTest()
        {
            CacheManager.Set("number", 12, CacheManager.GetProfile().WithGroup("testgroup"));
            int i = CacheManager.Get<int>("number", "testgroup");
            Assert.AreEqual(12, i);
        }

        [Test]
        public void InvalidateGroupTest()
        {
            CacheManager.Set("number", 12, CacheManager.GetProfile().WithGroup("testgroup"));
            int i = CacheManager.Get<int>("number", "testgroup");
            Assert.AreEqual(12, i);

            CacheManager.InvalidateGroup("testgroup");
            i = CacheManager.Get<int>("number", "testgroup");
            Assert.AreEqual(0, i);
        }

        [Test]
        public void ObjectTest()
        {
            var role = new Role() { Name = "Role1", Description = "Some description." };
            CacheManager.Set("role", role);
            Role cached = CacheManager.Get<Role>("role");
            Assert.AreEqual("Role1", cached.Name);
            Assert.AreSame(role, cached);
        }

        [Test]
        public void RemoveTest()
        {
            CacheManager.Set("number", 12);
            int i = CacheManager.Get<int>("number");
            Assert.AreEqual(12, i);

            bool success = CacheManager.Remove("number");
            Assert.IsTrue(success);

            success = CacheManager.Remove("number");
            Assert.IsFalse(success);

            i = CacheManager.Get<int>("number");
            Assert.AreEqual(0, i);
        }

        [Test]
        public void NumberPerfTest()
        {
            int count = 1000;
            var timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < count; i++)
            {
                CacheManager.Set("number", i);
                int cached = CacheManager.Get<int>("number");
                Assert.AreEqual(i, cached);
            }
            timer.Stop();
            double average = (double)timer.ElapsedMilliseconds / count;
            Console.WriteLine("Total: {0} Average: {1}ms", FormatTimeSpan(timer.Elapsed), average);
        }

        [Test]
        public void ObjectPerfTest()
        {
            int count = 1000;
            var timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < count; i++)
            {
                var role = new Role { Name = "Role" + i, Description = "Some description." };
                CacheManager.Set("role", role);
                Role cached = CacheManager.Get<Role>("role");
                Assert.AreEqual("Role" + i, cached.Name);
            }
            timer.Stop();
            double average = (double)timer.ElapsedMilliseconds / count;
            Console.WriteLine("Total: {0} Average: {1}ms", FormatTimeSpan(timer.Elapsed), average);
        }

        [Test]
        public void GetOrSet()
        {
            string key = "GetOrSet";
            string groupName = "testgroup";
            
            int i = CacheManager.Get<int>(key);
            Assert.AreEqual(0, i);

            i = CacheManager.GetOrSet(key, 12);
            Assert.AreEqual(12, i);

            i = CacheManager.Get<int>(key);
            Assert.AreEqual(12, i);            
        }

        [Test]
        public void GetOrSetGroup()
        {
            string key = "GetOrSetGroup";
            string groupName = "testgroup";

            int i = CacheManager.Get<int>(key, groupName);
            Assert.AreEqual(0, i);

            i = CacheManager.GetOrSet(key, 13, CacheManager.GetProfile().WithGroup(groupName));
            Assert.AreEqual(13, i);

            i = CacheManager.Get<int>(key, groupName);
            Assert.AreEqual(13, i);

            CacheManager.InvalidateGroup(groupName);
            i = CacheManager.Get<int>(key, groupName);
            Assert.AreEqual(0, i);

        }

        [Test]
        public void GetOrSetFactory()
        {
            string key = "GetOrSetFactory";
            int hashCode = key.GetHashCode();

            int i = CacheManager.Get<int>(key);
            Assert.AreEqual(0, i);

            i = CacheManager.GetOrSet(key, a => a.GetHashCode());

            Assert.AreEqual(hashCode, i);

            i = CacheManager.Get<int>(key);
            Assert.AreEqual(hashCode, i);
        }

        [Test]
        public void GetOrSetDataContext()
        {
            var db = new TrackerDataContext();

            var user = CacheManager.GetOrSet("william.adama@battlestar.com", k => db.User.GetByEmailAddress(k));

        }

        [Test]
        public void GetOrSetGroupFactory()
        {
            string key = "GetOrSetGroupFactory";
            string groupName = "testgroup";
            int hashCode = key.GetHashCode();

            int i = CacheManager.Get<int>(key, groupName);
            Assert.AreEqual(0, i);

            i = CacheManager.GetOrSet(key, a => a.GetHashCode(), CacheManager.GetProfile().WithGroup(groupName));
            Assert.AreEqual(hashCode, i);

            i = CacheManager.Get<int>(key, groupName);
            Assert.AreEqual(hashCode, i);

            CacheManager.InvalidateGroup(groupName);
            i = CacheManager.Get<int>(key, groupName);
            Assert.AreEqual(0, i);

        }
        private static string FormatTimeSpan(TimeSpan span)
        {
            var builder = new StringBuilder();
            if (span.Days > 0)
            {
                builder.Append(span.Days.ToString("00"));
                builder.Append(".");
            }
            if (span.Hours > 0 || builder.Length > 0)
            {
                builder.Append(span.Hours.ToString("00"));
                builder.Append(":");
            }
            if (span.Minutes > 0 || builder.Length > 0)
            {
                builder.Append(span.Minutes.ToString("00"));
                builder.Append(":");
            }

            builder.Append(span.Seconds.ToString("00"));
            builder.Append(".");
            builder.Append(span.Milliseconds.ToString("00"));

            return builder.ToString();
        }
    }
}