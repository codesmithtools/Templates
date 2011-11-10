using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tracker.Core;

namespace Tracker.Console
{
  class Program
  {
    static void Main(string[] args)
    {
      var db = new TrackerContext();

      var u = db.User.FirstOrDefault();

      u.Comment = "test:" + DateTime.Now.Ticks;

      db.SaveChanges();
    }
  }
}
