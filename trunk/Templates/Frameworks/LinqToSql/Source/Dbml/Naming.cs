using System;
using System.Collections.Generic;
using System.Text;

namespace LinqToSqlShared.DbmlObjectModel
{
    public static class Naming
    {

        public static string GetModifier(AccessModifier? access)
        {
            return access.ToString().ToLower();
        }

        public static string GetModifier(AccessModifier? access, ClassModifier? modifier)
        {
            if (modifier == null)
                return access.ToString().ToLower();
            else
                return string.Format("{0} {1}", access.ToString().ToLower(), modifier.ToString().ToLower());
        }

        public static string GetModifier(AccessModifier? access, MemberModifier? modifier)
        {
            if (modifier == null)
                return access.ToString().ToLower();
            else
                return string.Format("{0} {1}", access.ToString().ToLower(), modifier.ToString().ToLower());
        }

    }
}
