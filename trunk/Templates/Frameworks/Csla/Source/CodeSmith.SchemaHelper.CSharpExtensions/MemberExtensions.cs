using System;
using System.Collections.Generic;

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
					return string.Format(Environment.NewLine + "#if !SILVERLIGHT" + Environment.NewLine + "\t\t[System.ComponentModel.DataObjectField(true, {0})]" + Environment.NewLine + "#endif", member.IsIdentity.ToString().ToLower());
			
                return string.Format(Environment.NewLine + "\t\t[System.ComponentModel.DataObjectField(true, {0})]", member.IsIdentity.ToString().ToLower());
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
                        return Util.NamingConventions.VariableName(associationMember.ClassName);
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
                        return string.Format("{0}.{1}", Util.NamingConventions.VariableName(associationMember.ClassName), Util.NamingConventions.PropertyName(associationMember.ColumnName));
                    }
                }
            }

            return string.Empty;
        }
    }
}