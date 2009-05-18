using System;
using System.Collections.Generic;
using System.Text;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;
using NUnit.Framework;

namespace CodeSmith.Data.Tests.Rules.Assign
{
    [TestFixture]
    public class UserNameTests
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

            Assert.IsNotNull(current.CreatedBy);
            Assert.AreEqual(Environment.UserName, current.CreatedBy);

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

            Assert.IsNotNull(current.CreatedBy);
            Assert.AreEqual(Environment.UserName, current.CreatedBy);

        }

        [Test]
        public void EntityChangedModified()
        {
            RuleManager manager = new RuleManager();
            RuleManager.AddShared<TesterChanged>();

            var original = new TesterChanged();
            var current = new TesterChanged();
            current.CreatedBy = "blah";

            var trackedObject = new TrackedObject
            {
                Current = current,
                Original = original,
                IsChanged = true
            };

            manager.Run(trackedObject);

            Assert.IsNotNull(current.CreatedBy);
            Assert.AreEqual("blah", current.CreatedBy);
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

            Assert.IsNotNull(current.CreatedBy);
            Assert.AreEqual(Environment.UserName, current.CreatedBy);

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

            Assert.IsNotNull(current.CreatedBy);
            Assert.AreEqual(Environment.UserName, current.CreatedBy);

        }

        [Test]
        public void EntityNewModified()
        {
            RuleManager manager = new RuleManager();
            RuleManager.AddShared<TesterNew>();

            var original = new TesterNew();
            var current = new TesterNew();
            current.CreatedBy = "blah";

            var trackedObject = new TrackedObject
            {
                Current = current,
                Original = original,
                IsChanged = true
            };

            manager.Run(trackedObject);

            Assert.IsNotNull(current.CreatedBy);
            Assert.AreEqual("blah", current.CreatedBy);
        }

        public class TesterChanged
        {
            [UserName(EntityState.Changed)]
            public string CreatedBy { get; set; }
        }

        public class TesterNew
        {
            [UserName(EntityState.New)]
            public string CreatedBy { get; set; }
        }
    }


}
