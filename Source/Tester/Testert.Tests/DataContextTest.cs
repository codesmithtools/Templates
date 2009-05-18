using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Transactions;
using NUnit.Framework;
using Tester.Data;

namespace Testert.Tests
{
    [TestFixture]
    public class DataContextTest
    {

        [Test]
        public void MultipleResults()
        {
            var db = new TesterDataContext();
            var results = db.GetUser(1);
            Assert.IsNotNull(results);

            var user = results.GetResult<User>();
            Assert.IsNotNull(user);
            var userList = user.ToList();
            Assert.IsNotNull(userList);

            var userProfile = results.GetResult<UserProfile>();
            Assert.IsNotNull(userProfile);
            var userProfileList = userProfile.ToList();
            Assert.IsNotNull(userProfileList);

            var profile = db.GetUserProfile(1);
            Assert.IsNotNull(profile);

            var profileList = profile.ToList();
            Assert.IsNotNull(profileList);

        }



        [Test]
        public void UpdateBatch()
        {
            string modifiedBy = "t-" + DateTime.Now.Ticks;

            var db = new TesterDataContext();
            int count = db.Tag.Update(
                t => t.CreatedBy == "pwelter",
                t => new Tag { ModifiedBy = modifiedBy });

            Assert.GreaterOrEqual(count, 1);

            var tags = db.Tag.Where(t => t.CreatedBy == "pwelter").ToList();
            Assert.IsNotNull(tags);

            foreach (var tag in tags)
                Assert.AreEqual(modifiedBy, tag.ModifiedBy);
        }

        [Test]
        public void DeleteBatch()
        {
            string modifiedBy = "t-" + DateTime.Now.Ticks;

            var t1 = new Tag
             {
                 CreatedBy = modifiedBy,
                 Name = "Test1-" + DateTime.Now.Ticks,
                 Type = TagEnum.System,
                 ModifiedBy = modifiedBy
             };

            var t2 = new Tag
            {
                CreatedBy = modifiedBy,
                Name = "Test2-" + DateTime.Now.Ticks,
                Type = TagEnum.System,
                ModifiedBy = modifiedBy
            };

            var db = new TesterDataContext();
            db.Tag.InsertAllOnSubmit(new[] { t1, t2 });
            db.SubmitChanges();

            var tags = db.Tag.Where(t => t.CreatedBy == modifiedBy).ToList();
            Assert.IsNotNull(tags);
            Assert.AreEqual(2, tags.Count);

            int count = db.Tag.Delete(t => t.CreatedBy == modifiedBy);
            Assert.AreEqual(2, count);

            tags = db.Tag.Where(t => t.CreatedBy == modifiedBy).ToList();
            Assert.IsNotNull(tags);
            Assert.AreEqual(0, tags.Count);
        }

        [Test]
        public void DeleteByKey()
        {
            string modifiedBy = "t-" + DateTime.Now.Ticks;
            
            var t1 = new Tag
            {
                CreatedBy = modifiedBy,
                Name = "Test1-" + DateTime.Now.Ticks,
                Type = TagEnum.System,
                ModifiedBy = modifiedBy
            };

            var db = new TesterDataContext();
            db.Tag.InsertOnSubmit(t1);
            db.SubmitChanges();

            db.Tag.Delete(t1.Id);

        }

        [Test]
        public void ExecuteQuery()
        {
            var db = new TesterDataContext();

            var q1 = db.User.Where(u => u.EmailAddress == "admin@email.com");
            var q2 = db.Tag.Where(t => t.CreatedBy == "pwelter");
            var results = db.ExecuteQuery(q1, q2);

            var q3 = db.User
                .GetByEmailAddress("admin@email.com")
                .GetByPassword("blah")
                .GetByUserName("alala");

            var q4 = from b in db.User
                     where b.EmailAddress == "admin@email.com"
                        && (b.Password == "blah"
                        || b.UserName == "b")
                     select b;

            Assert.IsNotNull(results);

            var user = results.GetResult<User>();
            Assert.IsNotNull(user);
            var userList = user.ToList();
            Assert.IsNotNull(userList);

            var tag = results.GetResult<Tag>();
            Assert.IsNotNull(tag);
            var tags = tag.ToList();
            Assert.IsNotNull(tags);
        }

        [Test]
        public void TransactionExecuteQuery()
        {
            var db = new TesterDataContext();
            db.Log = Console.Out;

            db.Connection.Open();
            var tran = db.Connection.BeginTransaction();
            db.Transaction = tran;

            var q1 = db.User.Where(u => u.EmailAddress == "admin@email.com");
            var q2 = db.Tag.Where(t => t.CreatedBy == "pwelter");
            var results = db.ExecuteQuery(q1, q2);

            Assert.IsNotNull(results);

            var user = results.GetResult<User>();
            Assert.IsNotNull(user);
            var userList = user.ToList();
            Assert.IsNotNull(userList);

            var tag = results.GetResult<Tag>();
            Assert.IsNotNull(tag);
            var tags = tag.ToList();
            Assert.IsNotNull(tags);

            var tt = tags.FirstOrDefault();
            tt.IsBlah = (tt.IsBlah ?? 0) + 1;

            db.SubmitChanges();

            tran.Commit();
        }

        [Test]
        public void TransactionScopeExecuteQuery()
        {
            using (TransactionScope scope = new TransactionScope())
            {
                var db = new TesterDataContext();
                db.Log = Console.Out;

                var q1 = db.User.Where(u => u.EmailAddress == "admin@email.com");
                var q2 = db.Tag.Where(t => t.CreatedBy == "pwelter");
                var results = db.ExecuteQuery(q1, q2);

                Assert.IsNotNull(results);

                var user = results.GetResult<User>();
                Assert.IsNotNull(user);
                var userList = user.ToList();
                Assert.IsNotNull(userList);

                var tag = results.GetResult<Tag>();
                Assert.IsNotNull(tag);
                var tags = tag.ToList();
                Assert.IsNotNull(tags);

                var tt = tags.FirstOrDefault();
                tt.IsBlah = (tt.IsBlah ?? 0) + 1;

                db.SubmitChanges();

                scope.Complete();
            }
           
        }
    }
}
