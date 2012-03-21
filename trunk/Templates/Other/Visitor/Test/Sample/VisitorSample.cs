using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample
{
    public class VisitorSample
    {
        public Dictionary<string, VisitorChild> Dictionary { get; set; }
        public VisitorChildDictionary VisitorChildDictionary { get; set; }
        public List<string> StringList { get; set; }
        public List<VisitorChild> ChildList { get; set; }
        public VisitorChildCollection VisitorChildCollection { get; set; }
        public VisitorChild VisitorChild { get; set; }
    }

    public class VisitorChild
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public StringComparison StringComparison { get; set; }
        public int? Integer { get; set; }
        public DateTime DateTime { get; set; }

        public VisitorAddress HomeAddress { get; set; }
        public VisitorAddress BusinessAddress { get; set; }
    }

    public class VisitorAddress
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }

    public class VisitorChildCollection : List<VisitorChild>
    {

    }

    public class VisitorChildDictionary : Dictionary<string, VisitorChild>
    {

    }

}
