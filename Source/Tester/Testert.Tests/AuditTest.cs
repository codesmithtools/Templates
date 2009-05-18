using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Data.Linq;
using System.Text;
using CodeSmith.Data.Audit;
using NUnit.Framework;
using Tester.Data;
using AuditManager = CodeSmith.Data.Audit.AuditManager;

namespace Testert.Tests
{
    [TestFixture]
    public class AuditTest
    {
        [Test]
        public void EqualsTest()
        {
            AuditEntity e1 = new AuditEntity();
            AuditEntity e2 = new AuditEntity();
            AuditEntity e3 = new AuditEntity();
            AuditEntity e4 = new AuditEntity();
            AuditEntity e5 = new AuditEntity();

            e1.Action = AuditAction.Update;
            e1.Type = "Test";
            e1.Keys.Add(new AuditKey { Name = "Id", Type = "System.Int32", Value = 1 });

            e2.Action = AuditAction.Update;
            e2.Type = "Test";
            e2.Keys.Add(new AuditKey { Name = "Id", Type = "System.Int32", Value = 1 });

            e3.Action = AuditAction.Update;
            e3.Type = "Test";
            e3.Keys.Add(new AuditKey { Name = "Id", Type = "System.Int32", Value = 2 });

            e4.Action = AuditAction.Insert;
            e4.Type = "Test";
            e4.Keys.Add(new AuditKey { Name = "Id", Type = "System.Int32", Value = 1 });

            e5.Action = AuditAction.Update;
            e5.Type = "Tester";
            e5.Keys.Add(new AuditKey { Name = "Id", Type = "System.Int32", Value = 1 });

            Assert.AreEqual(e1.GetHashCode(), e2.GetHashCode());
            Assert.IsTrue(e1.Equals(e2));

            Assert.IsFalse(e1.Equals(e3));
            Assert.IsFalse(e1.Equals(e4));
            Assert.IsFalse(e1.Equals(e5));


            List<AuditEntity> auditEntities = new List<AuditEntity>();
            auditEntities.Add(e1);

            Assert.IsTrue(auditEntities.Contains(e2));
            Assert.IsFalse(auditEntities.Contains(e3));
            Assert.IsFalse(auditEntities.Contains(e4));
            Assert.IsFalse(auditEntities.Contains(e5));
        }

        [Test]
        public void Merge()
        {
            AuditEntity e1 = new AuditEntity { Action = AuditAction.Update, Type = "Test" };
            e1.Keys.Add(new AuditKey { Name = "Id", Type = "System.Int32", Value = 1 });
            e1.Properties.Add(new AuditProperty { Name = "Action", Type = "System.Int32", Current = 1, Original = 0});
            e1.Properties.Add(new AuditProperty { Name = "Name", Type = "System.String", Current = "B", Original = "A" });

            AuditEntity e2 = new AuditEntity { Action = AuditAction.Update, Type = "Test" };
            e2.Keys.Add(new AuditKey { Name = "Id", Type = "System.Int32", Value = 1 });
            e2.Properties.Add(new AuditProperty { Name = "Action", Type = "System.Int32", Current = 2, Original = 1 });
            e2.Properties.Add(new AuditProperty { Name = "Blah", Type = "System.String", Current = "D", Original = "C" });
            
            AuditEntity e3 = new AuditEntity { Action = AuditAction.Update, Type = "Test" };
            e3.Keys.Add(new AuditKey { Name = "Id", Type = "System.Int32", Value = 2 });
            e3.Properties.Add(new AuditProperty { Name = "Description", Type = "System.String", Current = "F", Original = "E" });
            
            AuditLog a1 = new AuditLog();
            a1.Entities.Add(e1);

            string xmlA1 = a1.ToXml();

            AuditLog a2 = new AuditLog();
            a2.Entities.Add(e2);
            a2.Entities.Add(e3);

            string xmlA2 = a2.ToXml();

            //true even though diff props
            Assert.IsTrue(e1.Equals(e2));
            
            AuditLog a3 = AuditManager.MergeAuditLogs(a1, a2);
            Assert.AreEqual(2, a3.Entities.Count);
            
            Assert.AreEqual(3, a3.Entities[0].Properties.Count);

            Assert.AreEqual(2, a3.Entities[0].Properties[0].Current);
            Assert.AreEqual(0, a3.Entities[0].Properties[0].Original);

            string xmlA3 = a3.ToXml();
        }

        [Test]
        public void AuditSetCreate()
        {
            var db = new TesterDataContext();
            var tags = db.Tag.GetByCreatedBy("pwelter");
            foreach (var tag in tags)
            {
                tag.Name = "tag-" + DateTime.Now.Ticks;
                tag.Type = TagEnum.System;
                tag.ModifiedDate = DateTime.Now;
            }

            var tag2 = new Tag();
            tag2.Name = "tag2";
            tag2.Type = TagEnum.User;
            tag2.CreatedBy = "pwelter";
            tag2.ModifiedBy = "pwelter";
            db.Tag.InsertOnSubmit(tag2);

            var user = db.User.GetByKey(1);
            user.Password = "password-" + DateTime.Now.Ticks;
            user.Comments = "comment " + DateTime.Now.Ticks;

            var newuser = new User();
            newuser.EmailAddress = "newuser@email.com";
            newuser.Password = "blah";
            newuser.UserName = "newuser";
            newuser.UserProfile = new UserProfile();
            newuser.UserProfile.AllowNotification = true;

            var ms = new MemoryStream();
            Properties.Resources.Image2.Save(ms, ImageFormat.Bmp);
            ms.Position = 0;

            newuser.UserProfile.Avatar = new Binary(ms.ToArray());
            db.User.InsertOnSubmit(newuser);

            var profile = db.UserProfile.GetByKey(1);
            db.UserProfile.DeleteOnSubmit(profile);

            var audit = AuditManager.CreateAuditLog(db);

            string xml = audit.ToXml();
            Console.WriteLine(xml);
        }

        [Test]
        public void SubmitAudit()
        {
            var db = new TesterDataContext();
            db.AuditingEnabled = true;

            var tags = db.Tag.GetByCreatedBy("pwelter");
            foreach (var tag in tags)
            {
                tag.Name = "tag-" + DateTime.Now.Ticks;
                tag.Type = TagEnum.System;
                tag.ModifiedDate = DateTime.Now;
            }

            var tag2 = new Tag();
            tag2.Name = "tag2";
            tag2.Type = TagEnum.User;
            tag2.CreatedBy = "pwelter";
            tag2.ModifiedBy = "pwelter";
            db.Tag.InsertOnSubmit(tag2);

            var user = db.User.GetByKey(1);
            user.Password = "password-" + DateTime.Now.Ticks;
            user.Comments = "comment " + DateTime.Now.Ticks;

            var newuser = new User();
            newuser.EmailAddress = "newuser@email.com";
            newuser.Password = "blah";
            newuser.UserName = "newuser";
            newuser.UserProfile = new UserProfile();
            newuser.UserProfile.AllowNotification = true;

            var ms = new MemoryStream();
            Properties.Resources.Image2.Save(ms, ImageFormat.Bmp);
            ms.Position = 0;

            newuser.UserProfile.Avatar = new Binary(ms.ToArray());
            db.User.InsertOnSubmit(newuser);

            db.SubmitChanges();

            Assert.IsNotNull(db.LastAudit);

            string xml = db.LastAudit.ToXml();
            Console.WriteLine(xml);


            //Audit audit = new Audit();
            //audit.Source = "Test";
            //audit.AuditXml = db.LastAudit.ToXml();

            //db.Audit.InsertOnSubmit(audit);
            //db.SubmitChanges();
        }
    }
}
