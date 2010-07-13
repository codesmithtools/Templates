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
            return member.BuildDataObjectField(false);
        }
		
		public static string BuildDataObjectField(this Member member, bool isSilverlight)
        {
            if (member.IsPrimaryKey)
            {
				if(isSilverlight)
                    return string.Format("{1}#if !SILVERLIGHT{1}\t\t[System.ComponentModel.DataObjectField(true, {0})]{1}#endif", member.IsIdentity.ToString().ToLower(), Environment.NewLine);

                return string.Format("{1}\t\t[System.ComponentModel.DataObjectField(true, {0})]", member.IsIdentity.ToString().ToLower(), Environment.NewLine);
            }

            return string.Empty;
        }

        /// <summary>
        /// This is used in the Insert and Update methods to figure out the association for the a fk member.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static string ResolveAssociationPropertyVariable(this Member member)
        {
            foreach (Association association in member.Entity.AssociatedManyToOne.Distinct())
            {
                foreach (AssociationMember associationMember in association)
                {
                    if (member.ColumnName == associationMember.AssociatedColumn.ColumnName && member.TableName == associationMember.AssociatedColumn.TableName)
                    {
                        var className = Util.NamingConventions.VariableName(associationMember.ClassName);
                        if (className.Equals("item", StringComparison.InvariantCultureIgnoreCase))
                            className += "1";

                        return className;
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// This is used in the AddNewCore methods to figure out the association for the a fk member.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static string ResolveAssociationPropertyClassName(this Member member)
        {
            foreach (Association association in member.Entity.AssociatedManyToOne.Distinct())
            {
                foreach (AssociationMember associationMember in association)
                {
                    if (member.ColumnName == associationMember.AssociatedColumn.ColumnName && member.TableName == associationMember.AssociatedColumn.TableName)
                    {
                        return Util.NamingConventions.PropertyName(associationMember.ClassName);
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// This is used in the Insert and Update methods to figure out the association for the a fk member.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static string ResolveAssociationPropertyVariableWithChildProperty(this Member member)
        {
            foreach (Association association in member.Entity.AssociatedManyToOne.Distinct())
            {
                foreach (AssociationMember associationMember in association)
                {
                    if (member.ColumnName == associationMember.AssociatedColumn.ColumnName && member.TableName == associationMember.AssociatedColumn.TableName)
                    {
                        var className = Util.NamingConventions.VariableName(associationMember.ClassName);
                        if (className.Equals("item", StringComparison.InvariantCultureIgnoreCase))
                            className += "1";

                        return string.Format("{0}.{1}", className, associationMember.MemberPropertyName);
                    }
                }
            }

            return string.Empty;
        }
    }
}