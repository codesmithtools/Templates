using System;
using System.Collections.Generic;
using System.Text;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;
using NUnit.Framework;

namespace CodeSmith.Data.Tests.Rules.Assign
{
    [TestFixture]
    public class NowTests
    {
        [Test]
        public void EntityChangedNotModified()
        {
            RuleManager manager = new RuleManager();
            RuleManager.AddShared<TesterChanged>();

            var original = new TesterChanged();
            var current = new TesterChanged();

            var trackedObject = new TrackedObject
            {
                Current = current,
                Original = original,
                IsChanged = true
            };

            manager.Run(trackedObject);

            Assert.AreNotEqual(DateTime.MinValue, current.CreatedOn);

        }

        [Test]
        public void EntityChangedNotModifiedNullOriginal()
        {
            RuleManager manager = new RuleManager();
            RuleManager.AddShared<TesterChanged>();

            var original = new TesterChanged();
            var current = new TesterChanged();

            var trackedObject = new TrackedObject
            {
                Current = current,
                Original = null,
                IsChanged = true
            };

            manager.Run(trackedObject);

            Assert.AreNotEqual(DateTime.MinValue, current.CreatedOn);
        }

        [Test]
        public void EntityChangedModified()
        {
            RuleManager manager = new RuleManager();
            RuleManager.AddShared<TesterChanged>();

            var original = new TesterChanged();
            var current = new TesterChanged();

            DateTime assigned = new DateTime(2009, 1, 1);
            current.CreatedOn = assigned;

            var trackedObject = new TrackedObject
            {
                Current = current,
                Original = original,
                IsChanged = true
            };

            manager.Run(trackedObject);

            Assert.AreNotEqual(DateTime.MinValue, current.CreatedOn);
            Assert.AreEqual(assigned, current.CreatedOn);
        }

        [Test]
        public void EntityNewNotModified()
        {
            RuleManager manager = new RuleManager();
            RuleManager.AddShared<TesterNew>();

            var original = new TesterNew();
            var current = new TesterNew();

            var trackedObject = new TrackedObject
            {
                Current = current,
                Original = original,
                IsNew = true
            };

            manager.Run(trackedObject);

            Assert.AreNotEqual(DateTime.MinValue, current.CreatedOn);

        }

        [Test]
        public void EntityNewNotModifiedNullOriginal()
        {
            RuleManager manager = new RuleManager();
            RuleManager.AddShared<TesterNew>();

            var original = new TesterNew();
            var current = new TesterNew();

            var trackedObject = new TrackedObject
            {
                Current = current,
                Original = null,
                IsNew = true
            };

            manager.Run(trackedObject);

            Assert.AreNotEqual(DateTime.MinValue, current.CreatedOn);

        }

        [Test]
        public void EntityNewModified()
        {
            RuleManager manager = new RuleManager();
            RuleManager.AddShared<TesterNew>();

            var original = new TesterNew();
            var current = new TesterNew();
            DateTime assigned = new DateTime(2009, 1, 1);
            current.CreatedOn = assigned;

            var trackedObject = new TrackedObject
            {
                Current = current,
                Original = original,
                IsNew = true
            };

            manager.Run(trackedObject);

            Assert.AreNotEqual(DateTime.MinValue, current.CreatedOn);
            Assert.AreEqual(assigned, current.CreatedOn);
        }

        [Test]
        public void EntityNewIsChanged()
        {
            RuleManager manager = new RuleManager();
            RuleManager.AddShared<TesterNew>();

            var original = new TesterNew();
            var current = new TesterNew();

            var trackedObject = new TrackedObject
            {
                Current = current,
                Original = original,
                IsChanged = true
            };

            manager.Run(trackedObject);

            Assert.AreEqual(DateTime.MinValue, current.CreatedOn);
        }

        public class TesterChanged
        {
            [Now(EntityState.Changed)]
            public DateTime CreatedOn { get; set; }
        }

        public class TesterNew
        {
            [Now(EntityState.New)]
            public DateTime CreatedOn { get; set; }
        }
    }
}
