using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Text;
using EntityFramework.Extensions;
using Tracker.Core;
using Tracker.Entity;

namespace Tracker.Console
{
  class Program
  {
    static void Main(string[] args)
    {
      Expression<Func<int, int, int>> expression = (a, b) => a + b;

      var db = new TrackerEntities();
      string value = "@test.com";
      var q = db.Users.Where(u => u.Email.EndsWith(value));
      var e = q.Expression as MethodCallExpression;
      var o = q as ObjectQuery<User>;
      string sql = o.ToTraceString();
      var p = o.Parameters;

      
      var q2 = o.Select(s => s.Id);
      var e2 = q2.AsQueryable().Expression;
      var o2 = q2 as ObjectQuery<int>;
      string sql2 = o2.ToTraceString();




    }
  }

}

