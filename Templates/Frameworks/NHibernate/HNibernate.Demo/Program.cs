using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate.Generated;
using NHibernate.Generated.ManagerObjects;
using NHibernate.Generated.BusinessObjects;

namespace NHibernate.Demo
{
    class Program
    {
        static ManagerFactory ManagerFactory = new ManagerFactory();
        static MockManagerFactory MockManagerFactory = new MockManagerFactory();

        static void Main(string[] args)
        {
            MockUnitTester();

            Console.Out.WriteLine("Done");
            Console.In.ReadLine();
        }

        static void MockUnitTester()
        {
            IPersonManager pm = MockManagerFactory.GetPersonManager();

            //Person p = new Person();
            //p.Name = "Testie McTester";
            //p.Id = 1;
            //pm.Save(p);

            Person p1 = pm.GetById(1);
            Person p2 = pm.GetById(2);
            IList<Person> pList = pm.GetByCriteria();

        }

        static void UnitTester()
        {
            NHibernate.Base.NHibernateSessionManager.Instance = new UnitTestSessionManager();

            IPersonManager pm = ManagerFactory.GetPersonManager();
            IList<Person> pList = pm.GetAll();

            Person p = new Person();
            p.Name = "Testie McTester";

            pm.Save(p);
            pm.Session.CommitChanges();

            pList = pm.GetAll();
        }
    }
}
