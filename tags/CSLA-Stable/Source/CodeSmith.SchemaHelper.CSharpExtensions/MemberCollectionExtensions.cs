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
        #region BuildObjectInitializer

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
                if(member.IsNullable && member.SystemType != "System.String" && member.SystemType != "System.Byte[]") continue;

                var propertyName = isObjectFactory ? 
                                string.Format("item.{0}", member.PropertyName) : 
                                usePropertyName ? member.PropertyName : member.VariableName;

                if (includeOriginal && member.IsPrimaryKey && !member.IsIdentity)
                    propertyName = isObjectFactory ? string.Format("item.Original{0}", member.PropertyName) : string.Format("Original{0}", member.PropertyName);
                
                parameters += string.Format(", {0} = {1}", member.PropertyName, propertyName);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        #endregion

        #region BuildNullableObjectInitializer

        public static string BuildNullableObjectInitializer(this List<Member> members)
        {
            return members.BuildNullableObjectInitializer(false);
        }

        public static string BuildNullableObjectInitializer(this List<Member> members, bool isObjectFactory)
        {
            return members.BuildNullableObjectInitializer(isObjectFactory, false);
        }

        public static string BuildNullableObjectInitializer(this List<Member> members, bool isObjectFactory, bool usePropertyName)
        {
            return members.BuildNullableObjectInitializer(isObjectFactory, usePropertyName, false);
        }

        public static string BuildNullableObjectInitializer(this List<Member> members, bool isObjectFactory, bool usePropertyName, bool includeOriginal)
        {
            return members.BuildNullableObjectInitializer(isObjectFactory, usePropertyName, includeOriginal, "criteria.");
        }

        public static string BuildNullableObjectInitializer(this List<Member> members, bool isObjectFactory, bool usePropertyName, bool includeOriginal, string prefix)
        {
            string parameters = string.Empty;

            foreach (Member member in members)
            {
                if ((member.IsNullable && member.SystemType != "System.String" && member.SystemType != "System.Byte[]") == false) continue;

                var propertyName = isObjectFactory ?
                                string.Format("item.{0}", member.PropertyName) :
                                usePropertyName ? member.PropertyName : member.VariableName;

                if (includeOriginal && member.IsPrimaryKey && !member.IsIdentity)
                    propertyName = isObjectFactory ? string.Format("item.Original{0}", member.PropertyName) : string.Format("Original{0}", member.PropertyName);

                parameters += string.Format("\r\n\t\t\t\tif({1}.HasValue) {2}{0} = {1}.Value;", member.PropertyName, propertyName, prefix);
            }

            return parameters.TrimStart(new[] { '\r', '\n', '\t' });
        }

        #endregion

        #region BuildParametersVariables

        public static string BuildParametersVariables(this List<Member> members)
        {
            return members.BuildParametersVariables(true);
        }

        public static string BuildParametersVariables(this List<Member> members, bool isNullable)
        {
            string parameters = string.Empty;

            foreach (Member member in members)
            {
                string systemType = isNullable ? member.SystemType : member.SystemType.TrimEnd(new[] {'?'});
                parameters += string.Format(", {0} {1}", systemType, member.VariableName);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        #endregion

        #region BuildCommandParameters

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
                string includeThisPrefix = !isObjectFactory ? "this." : string.Empty;
                string propertyName = member.PropertyName;
                string originalPropertyName = string.Format("Original{0}", member.PropertyName);
                
                // Resolve property Name from relationship.
                if (isChildInsertUpdate && member.IsForeignKey)
                {
                    foreach (Association association in member.Entity.AssociatedManyToOne)
                    {
                        foreach (AssociationMember associationMember in association)
                        {
                            if (member.ColumnName == associationMember.AssociatedColumn.ColumnName && member.TableName == associationMember.AssociatedColumn.TableName)
                            {
                                propertyName = string.Format("{0}.{1}", Util.NamingConventions.VariableName(associationMember.ClassName), associationMember.MemberPropertyName);

                                var format = associationMember.IsPrimaryKey && !associationMember.IsIdentity ? "{0}.Original{1}" : "{0}.{1}";
                                originalPropertyName = string.Format(format, Util.NamingConventions.VariableName(associationMember.ClassName), associationMember.MemberPropertyName);

                                className = Util.NamingConventions.VariableName(associationMember.ClassName);
                                includeThisPrefix = string.Empty;
                                break;
                            }
                        }
                    }
                }

                var nullableType = string.Format("{0}{1}", !isObjectFactory ? "this." : string.Empty, member.PropertyName);
                var originalNullableType = string.Format("{0}Original{1}", !isObjectFactory ? "this." : string.Empty, member.PropertyName);
                //var nullableType = string.Format("new {0}()", member.SystemType);
                //if (member.SystemType == "System.String" || member.SystemType == "System.Byte[]")
                //    nullableType = "null";

                string originalCast;
                string cast;
                if (member.IsNullable && member.SystemType != "System.Byte[]")
                {
                    //includeThisPrefix = this.
                    //castprefix = item.
                    //propertyName = bo.propertyname or propertyname
                    if (!string.IsNullOrEmpty(className))
                    {
                        cast = string.Format("ADOHelper.NullCheck({3} != null ? {0}{1}{2} : {4}));", includeThisPrefix, castPrefix, propertyName, className, nullableType);
                        originalCast = string.Format("ADOHelper.NullCheck({3} != null ? {0}{1}{2} : {4}));", includeThisPrefix, castPrefix, originalPropertyName, className, originalNullableType);
                    }
                    else
                    {
                        cast = string.Format("ADOHelper.NullCheck({0}{1}{2}));", includeThisPrefix, castPrefix, propertyName);
                        originalCast = string.Format("ADOHelper.NullCheck({0}{1}{2}));", includeThisPrefix, castPrefix, originalPropertyName);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(className))
                    {
                        cast = string.Format("{3} != null ? {0}{1}{2} : {4});", includeThisPrefix, castPrefix, propertyName, className, nullableType);
                        originalCast = string.Format("{3} != null ? {0}{1}{2} : {4});", includeThisPrefix, castPrefix, originalPropertyName, className, originalNullableType);
                    }
                    else
                    {
                        cast = string.Format("{0}{1}{2});", includeThisPrefix, castPrefix, propertyName);
                        originalCast = string.Format("{0}{1}{2});", includeThisPrefix, castPrefix, originalPropertyName);
                    }
                }

                bool includeOriginalPropertyName = isUpdateStatement && member.IsPrimaryKey && !member.IsIdentity;
                if (isUpdateStatement && includeOriginalPropertyName)
                    commandParameters += string.Format(Environment.NewLine + "\t\t\t\t\tcommand.Parameters.AddWithValue(\"{0}Original{1}\", {2}", Configuration.Instance.ParameterPrefix, member.ColumnName, originalCast);

                commandParameters += string.Format(Environment.NewLine + "\t\t\t\t\tcommand.Parameters.AddWithValue(\"{0}{1}\", {2}", Configuration.Instance.ParameterPrefix, member.ColumnName, cast);

                if ((member.IsIdentity || (member.DataType == DbType.Guid.ToString() && member.IsPrimaryKey && !member.IsForeignKey)) && includeOutPutParameters)
                {
                    if (isUpdateStatement)
                        commandParameters += string.Format(Environment.NewLine + "\t\t\t\t\tcommand.Parameters[\"{0}{1}\"].Direction = ParameterDirection.Input;", Configuration.Instance.ParameterPrefix, member.ColumnName);
                    else
                        commandParameters += string.Format(Environment.NewLine + "\t\t\t\t\tcommand.Parameters[\"{0}{1}\"].Direction = ParameterDirection.Output;", Configuration.Instance.ParameterPrefix, member.ColumnName);
                }
            }

            return commandParameters.TrimStart(new[] { '\t', '\r', '\n' });
        }

        #endregion

        public static string BuildHasValueCommandParameters(this List<Member> members)
        {
            string commandParameters = string.Empty;

            foreach (Member member in members)
            {
                if(member.IsNullable)
                    commandParameters += string.Format(Environment.NewLine + "\t\t\t\t\tcommand.Parameters.AddWithValue(\"{0}{1}HasValue\", criteria.{2}HasValue);", Configuration.Instance.ParameterPrefix, member.ColumnName, member.PropertyName);
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
                    statement += string.Format(" || {1}Original{0} != {1}{0}", member.PropertyName, prefix);
            }

            return statement.TrimStart(new[] { '|', ' ' });
        }
    }
}
