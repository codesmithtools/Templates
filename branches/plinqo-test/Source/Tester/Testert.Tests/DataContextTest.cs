using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public void MyTest()
        {
            var db = new TesterDataContext();
            db.Log = Console.Out;

            var audit = db.Audit.FirstOrDefault();

            var xml = audit.AuditXml;


        }
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

        [Test]
        public void ProgrammerInheritance()
        {
            var db = new TesterDataContext();
            db.Log = Console.Out;

            var query = from p in db.Person.OfType<Programmer>()
                        select p;

            int i = 1;
            foreach (var programmer in query)
            {
                programmer.Language = "c#";
                programmer.Level = string.Format("Level {0}", i++);
            }

            db.SubmitChanges();

            var p1 = new Programmer();
            p1.FirstName = "Bill";
            p1.LastName = "Coder";
            p1.JobTitle = "Coder";
            p1.Language = "c#";

            
            db.Person.InsertOnSubmit(p1);

            db.SubmitChanges();

        }


        [Test]
        public void ProgrammerInheritanceAudit()
        {
            var db = new TesterDataContext();
            db.Log = Console.Out;
            db.AuditingEnabled = true;

            var programmer = db.Person.OfType<Programmer>().FirstOrDefault();
            programmer.Language = "c#";
            programmer.Level = string.Format("Level {0}", DateTime.Now.Ticks);
            programmer.FirstName = "Name " + DateTime.Now.Ticks;

            db.SubmitChanges();

            var a = db.LastAudit;
            Console.WriteLine(a.ToXml());

            var p1 = new Programmer();
            p1.FirstName = "Bob";
            p1.LastName = "Coder";
            p1.JobTitle = "Coder";
            p1.Language = "c#";

            db.Person.InsertOnSubmit(p1);

            db.SubmitChanges();
            a = db.LastAudit;
            Console.WriteLine(a.ToXml());
        }

        [Test]
        public void ManyToManyDelete()
        {
            var db = new TesterDataContext();
            db.Log = Console.Out;

            var left = db.Left.GetByKey(1);
            var right = db.Right.GetByDescription("This is new").FirstOrDefault();

            //var right = new Right();
            //right.Description = "This is new";
            
            //db.Right.InsertOnSubmit(right);
            
            left.RightList.Add(right);

            db.SubmitChanges();

            left.RightList.Remove(right);

            db.SubmitChanges();
            
        }
    }
}
