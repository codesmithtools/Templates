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

        public static bool CanGenerateNullDefault(this Member member)
        {
            bool defaultValueAllowed = member.BaseSystemType == "System.Xml.XmlDocument" ||
                                       member.BaseSystemType == "System.Guid" ||
                                       member.BaseSystemType == "System.Byte" ||
                                       member.SystemType == "System.Byte()" ||
                                       member.BaseSystemType == "System.DateTime" ||
                                       member.BaseSystemType == "System.DateTimeOffset" ||
                                       member.BaseSystemType == "System.Boolean";

            if (member.IsNullable && !defaultValueAllowed)
            {
                return true;
            }

            return false;
        }
    }
}