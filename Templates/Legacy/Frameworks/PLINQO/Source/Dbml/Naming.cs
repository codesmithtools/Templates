using System;

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
            return !modifier.HasValue
                       ? access.ToString().ToLower()
                       : String.Format("{0} {1}", access.ToString().ToLower(), modifier.ToString().ToLower());
        }

        public static string GetModifier(AccessModifier? access, MemberModifier? modifier)
        {
            return !modifier.HasValue
                       ? access.ToString().ToLower()
                       : String.Format("{0} {1}", access.ToString().ToLower(), modifier.ToString().ToLower());
        }

        public static string GetVisualBasicModifier(AccessModifier? access)
        {
            return GetVisualBasicModifier(access.ToString());
        }

        public static string GetVisualBasicModifier(AccessModifier? access, ClassModifier? modifier)
        {
            return !modifier.HasValue
                       ? GetVisualBasicModifier(access.ToString())
                       : String.Format("{0} {1}", GetVisualBasicModifier(access.ToString()), modifier);
        }

        public static string GetVisualBasicModifier(AccessModifier? access, MemberModifier? modifier)
        {
            return !modifier.HasValue
                       ? GetVisualBasicModifier(access.ToString())
                       : String.Format("{0} {1}", GetVisualBasicModifier(access.ToString()), modifier);
        }

        private static string GetVisualBasicModifier(string modifier)
        {
            if (String.Equals(modifier, "Internal", StringComparison.OrdinalIgnoreCase))
                return "Friend";

            if (String.Equals(modifier, "ProtectedInternal", StringComparison.OrdinalIgnoreCase))
                return "Protected Friend";

            return modifier;
        }
    }
}
