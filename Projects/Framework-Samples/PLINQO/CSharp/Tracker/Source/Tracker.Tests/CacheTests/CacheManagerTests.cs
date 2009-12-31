using System;
using System.Diagnostics;
using System.Text;
using CodeSmith.Data.Caching;
using NUnit.Framework;
using Tracker.Core.Data;

namespace Tracker.Tests.CacheTests
{
    [TestFixture]
    public class CacheManagerTests : RoleTests
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
            Console.WriteLine("Total: " + FormatTimeSpan(timer.Elapsed) + " Average: " + average + "ms");
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
            Console.WriteLine("Total: " + FormatTimeSpan(timer.Elapsed) + " Average: " + average + "ms");
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