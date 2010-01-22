using System;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for MemberExtensions
    /// </summary>
    public static class MemberExtensions
    {
        public static string BuildDataObjectField(this Member member)
        {
            if (member.IsPrimaryKey)
            {
                return string.Format("\n\t\t<System.ComponentModel.DataObjectField(true, {0})> _", member.IsIdentity.ToString().ToLower());
            }

            return string.Empty;
        }
    }
}