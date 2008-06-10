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

        static void Main(string[] args)
        {
            NHibernateExample();

            Console.Out.WriteLine("Done");
            Console.In.ReadLine();
        }

        /// <summary>
        /// This method was written to show what your code would look like if you had a Person table with a Name column.
        /// </summary>
        static void NHibernateExample()
        {
        //    ManagerFactory managerFactory = new ManagerFactory();
        //    IPersonManager personManager = managerFactory.GetPersonManager();
        //    Person person = personManager.GetById(1);
        //    Console.Out.WriteLine("Person {0}'s name is {1}.", person.Id, person.Name);
        }
    }
}
