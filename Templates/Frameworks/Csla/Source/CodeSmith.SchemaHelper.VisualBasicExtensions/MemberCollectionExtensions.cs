using System;
using System.Collections.Generic;
using System.Data;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for MemberCollectionExtensions
    /// </summary>
    public static class MemberCollectionExtensions
    {
        public static string BuildObjectInitializer(this List<Member> members)
        {
            return members.BuildObjectInitializer(false);
        }

        public static string BuildObjectInitializer(this List<Member> members, bool isObjectFactory)
        {
            return members.BuildObjectInitializer(isObjectFactory, false);
        }

        public static string BuildObjectInitializer(this List<Member> members, bool isObjectFactory, bool usePropertyName)
        {
            return members.BuildObjectInitializer(isObjectFactory, usePropertyName, false);
        }
        
        public static string BuildObjectInitializer(this List<Member> members, bool isObjectFactory, bool usePropertyName, bool includeOriginal)
        {
            return members.BuildObjectInitializer(isObjectFactory, usePropertyName, includeOriginal, "criteria.");
        }
        
        public static string BuildObjectInitializer(this List<Member> members, bool isObjectFactory, bool usePropertyName, bool includeOriginal, string prefix)
        {
            string parameters = string.Empty;

            foreach (Member member in members)
            {
                var propertyName = isObjectFactory ? 
                                string.Format("item.{0}", member.PropertyName) : 
                                usePropertyName ? member.PropertyName : member.VariableName;

                if (includeOriginal && member.IsPrimaryKey && !member.IsIdentity)
                    propertyName = isObjectFactory ? string.Format("item.Original{0}", member.PropertyName) : string.Format("Original{0}", member.PropertyName);
                parameters += string.Format("\r\n\t\t\t{3}{0} = {1}{2}", member.PropertyName, propertyName, member.IsNullable && member.SystemType != "System.String" && member.SystemType != "System.Byte()" ? ".Value" : string.Empty, prefix);
            }

            return parameters.TrimStart(new[] { '\r', '\n', '\t', ',', ' ' });
        }

        public static string BuildParametersVariables(this List<Member> members)
        {
            return members.BuildParametersVariables(true);
        }

        public static string BuildParametersVariables(this List<Member> members, bool isNullable)
        {
            string parameters = string.Empty;

            foreach (Member member in members)
            {
                string systemType = isNullable ? member.SystemType : member.SystemType.TrimEnd(new[] { '?' });
                parameters += string.Format(", ByVal {0} As {1}", member.VariableName, systemType);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildCommandParameters(this List<Member> members)
        {
            return BuildCommandParameters(members, false);
        }

        public static string BuildCommandParameters(this List<Member> members, bool isObjectFactory)
        {
            return BuildCommandParameters(members, isObjectFactory, false);
        }

        public static string BuildCommandParameters(this List<Member> members, bool isObjectFactory, bool usePropertyName)
        {
            return BuildCommandParameters(members, isObjectFactory, usePropertyName, false);
        }

        public static string BuildCommandParameters(this List<Member> members, bool isObjectFactory, bool usePropertyName, bool isChildInsertUpdate)
        {
            return members.BuildCommandParameters(isObjectFactory, usePropertyName, isChildInsertUpdate, false);
        }

        public static string BuildCommandParameters(this List<Member> members, bool isObjectFactory, bool usePropertyName, bool isChildInsertUpdate, bool includeOutPutParameters)
        {
            return members.BuildCommandParameters(isObjectFactory, usePropertyName, isChildInsertUpdate, includeOutPutParameters, false);
        }

        public static string BuildCommandParameters(this List<Member> members, bool isObjectFactory, bool usePropertyName, bool isChildInsertUpdate, bool includeOutPutParameters, bool isUpdateStatement)
        {
            string commandParameters = string.Empty;
            string castPrefix = isObjectFactory ? "item." : string.Empty;

            foreach (Member member in members)
            {
                string className = string.Empty;
                string includeThisPrefix = !isObjectFactory ? "Me." : string.Empty;
                string originalPropertyName = isUpdateStatement && member.IsPrimaryKey && !member.IsIdentity ? string.Format("Original{0}", member.PropertyName) : string.Empty;
                string propertyName = member.PropertyName;
                
                // Resolve property Name from relationship.
                if(isChildInsertUpdate && member.IsForeignKey)
                {
                    foreach (Association association in member.Entity.AssociatedManyToOne)
                    {
                        foreach (AssociationMember associationMember in association)
                        {
                            if (member.ColumnName == associationMember.AssociatedColumn.ColumnName && member.TableName == associationMember.AssociatedColumn.TableName)
                            {
                                propertyName = string.Format("{0}.{1}", Util.NamingConventions.VariableName(associationMember.ClassName), Util.NamingConventions.PropertyName(associationMember.ColumnName));
                                className = Util.NamingConventions.VariableName(associationMember.ClassName);
                                includeThisPrefix = string.Empty; 
                                break;
                            }
                        }   
                    }
                }

                var nullableType = string.Format("{0}{1}", !isObjectFactory ? "Me." : string.Empty, member.PropertyName);
                //var nullableType = string.Format("New {0}()", member.SystemType);
                //if (member.SystemType == "System.String" || member.SystemType == "System.Byte()")
                //    nullableType = "Nothing";

                string originalCast;
                string cast;
                if (member.IsNullable && member.SystemType != "System.Byte()")
                {
                    //includeThisPrefix = this.
                    //castprefix = item.
                    //propertyName = bo.propertyname or propertyname
                    if (!string.IsNullOrEmpty(className))
                    {
                        cast = string.Format("ADOHelper.NullCheck(If(Not({3} Is Nothing), {0}{1}{2}, {4})))", includeThisPrefix, castPrefix, propertyName, className, nullableType);
                        originalCast = string.Format("ADOHelper.NullCheck(If(Not({3} Is Nothing), {0}{1}{2}, {4})))", includeThisPrefix, castPrefix, originalPropertyName, className, nullableType);
                    }
                    else
                    {
                        cast = string.Format("ADOHelper.NullCheck({0}{1}{2}))", includeThisPrefix, castPrefix, propertyName);
                        originalCast = string.Format("ADOHelper.NullCheck({0}{1}{2}))", includeThisPrefix, castPrefix, originalPropertyName);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(className))
                    {
                        cast = string.Format("If(Not({3} Is Nothing), {0}{1}{2}, {4}))", includeThisPrefix, castPrefix, propertyName, className, nullableType);
                        originalCast = string.Format("If(Not({3} Is Nothing), {0}{1}{2}, {4}))", includeThisPrefix, castPrefix, originalPropertyName, className, nullableType);
                    }
                    else
                    {
                        cast = string.Format("{0}{1}{2})", includeThisPrefix, castPrefix, propertyName);
                        originalCast = string.Format("{0}{1}{2})", includeThisPrefix, castPrefix, originalPropertyName);
                    }
                }

                if (isUpdateStatement && !string.IsNullOrEmpty(originalPropertyName))
                    commandParameters += string.Format(Environment.NewLine + "\t\t\t\tcommand.Parameters.AddWithValue(\"{0}Original{1}\", {2}", Configuration.Instance.ParameterPrefix, member.ColumnName, originalCast);
                
                commandParameters += string.Format(Environment.NewLine + "\t\t\t\tcommand.Parameters.AddWithValue(\"{0}{1}\", {2}", Configuration.Instance.ParameterPrefix, member.ColumnName, cast);

                if ((member.IsIdentity || (member.DataType == DbType.Guid.ToString() && member.IsPrimaryKey)) && includeOutPutParameters)
                {
                    commandParameters += string.Format(Environment.NewLine + "\t\t\t\t\tcommand.Parameters(\"{0}{1}\").Direction = ParameterDirection.Output", Configuration.Instance.ParameterPrefix, member.ColumnName);
                }
            }

            return commandParameters.TrimStart(new[] { '\t', '\r', '\n' });
        }

        public static string BuildHasValueCommandParameters(this List<Member> members)
        {
            string commandParameters = string.Empty;

            foreach (Member member in members)
            {
                if (member.IsNullable)
                    commandParameters += string.Format(Environment.NewLine + "\t\t\t\tcommand.Parameters.AddWithValue(\"{0}{1}HasValue\", criteria.{2}HasValue)", Configuration.Instance.ParameterPrefix, member.ColumnName, member.PropertyName);
            }

            return commandParameters.TrimStart(new[] { '\t', '\r', '\n' });
        }

        public static string BuildIdentityKeyEqualityStatements(this List<Member> members)
        {
            return members.BuildIdentityKeyEqualityStatements("");
        }

        public static string BuildIdentityKeyEqualityStatements(this List<Member> members, string prefix)
        {
            string statement = string.Empty;

            foreach (Member member in members)
            {
                if(member.IsPrimaryKey && !member.IsIdentity)
                    statement += string.Format(" Or Not {1}Original{0} = {1}{0}", member.PropertyName, prefix);
            }

            return statement.Substring(3, statement.Length - 3).TrimStart(new[] { ' ' });
        }
    }
}
