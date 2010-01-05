using System;
using System.Collections.Generic;
using System.Linq;
using CodeSmith.SchemaHelper.Util;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for MemberCollectionExtensions
    /// </summary>
    public static class MemberCollectionExtensions
    {
        public static List<SearchCriteria> ListSearchCriteria(this List<Member> members)
        {
            List<SearchCriteria> searchCriterias = new List<SearchCriteria>();

            foreach (var member in members)
            {
                string memberName = member.Name;
                searchCriterias.AddRange(member.SearchCriteria.Where(sc => !sc.IsUniqueResult && sc.MethodName.Contains(memberName)));
            }

            return searchCriterias;
        }

        public static string BuildObjectInitializer(this List<Member> members)
        {
            string parameters = string.Empty;

            foreach (Member member in members)
            {
                parameters += string.Format(", {0} = {1}", member.PropertyName, member.VariableName);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildParametersVariables(this List<Member> members)
        {
            return members.BuildParametersVariables(true);
        }

        public static string BuildParametersVariables(this List<Member> members, bool isNullable)
        {
            return members.BuildParametersVariables(new List<AssociationMember>(), isNullable);
        }

        public static string BuildParametersVariables(this List<Member> members, List<AssociationMember> associationMembers)
        {
            return members.BuildParametersVariables(associationMembers, true);
        }

        public static string BuildParametersVariables(this List<Member> members, List<AssociationMember> associationMembers, bool isNullable)
        {
            string parameters = string.Empty;

            foreach (Member member in members)
            {
                string systemType = isNullable ? member.SystemType : member.SystemType.TrimEnd(new[] { '?' });

                if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
                    parameters += string.Format(", ByVal {0} As {1}", member.VariableName, systemType);
                else
                    parameters += string.Format(", {0} {1}", systemType, member.VariableName);
            }

            foreach (AssociationMember associationMember in associationMembers)
            {
                string systemType = isNullable
                                        ? associationMember.LocalColumn.SystemType
                                        : associationMember.LocalColumn.SystemType.TrimEnd(new[] {'?'});

                string output = string.Empty;;

                if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
                    output = string.Format(", ByVal {0} As {1}", NamingConventions.VariableName(associationMember.ColumnName), systemType);
                else
                    output = string.Format(", {0} {1}", systemType, NamingConventions.VariableName(associationMember.ColumnName));

                if (!parameters.Contains(output))
                    parameters += output;
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildParametersVariablesCriteria(this List<Member> members)
        {
            return members.BuildParametersVariablesCriteria(true);
        }

        public static string BuildParametersVariablesCriteria(this List<Member> members, bool isNullable)
        {
            string parameters = string.Empty;

            foreach (Member member in members)
            {
                string systemType = isNullable ? member.SystemType : member.SystemType.TrimEnd(new[] { '?' });

                if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
                    parameters += string.Format(", ByVal {0} As {1}", member.Entity.ResolveCriteriaVariableName(member.ColumnName), systemType);
                else
                    parameters += string.Format(", {0} {1}", systemType, member.Entity.ResolveCriteriaVariableName(member.ColumnName));
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildInsertParametersVariables(this List<Member> members)
        {
            return members.BuildInsertParametersVariables(true);
        }

        public static string BuildInsertParametersVariables(this List<Member> members, bool isNullable)
        {
            return members.BuildInsertParametersVariables(new List<AssociationMember>(), isNullable);
        }

        public static string BuildInsertParametersVariables(this List<Member> members, List<AssociationMember> associationMembers)
        {
            return members.BuildInsertParametersVariables(associationMembers, true);
        }

        public static string BuildInsertParametersVariables(this List<Member> members, List<AssociationMember> associationMembers, bool isNullable)
        {
            string parameters = string.Empty;

            foreach (Member member in members)
            {
                if (!member.IsIdentity)
                {
                    string systemType = isNullable ? member.SystemType : member.SystemType.TrimEnd(new[] { '?' });

                    if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
                        parameters += string.Format(", ByVal {0} As {1}", member.VariableName, systemType);
                    else
                        parameters += string.Format(", {0} {1}", systemType, member.VariableName);
                }
            }

            foreach (AssociationMember associationMember in associationMembers)
            {
                if (!associationMember.IsIdentity)
                {
                    string systemType = isNullable
                                            ? associationMember.LocalColumn.SystemType
                                            : associationMember.LocalColumn.SystemType.TrimEnd(new[] {'?'});
                    
                    string output = string.Empty;

                    if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
                        output = string.Format(", ByVal {0} As {1}", NamingConventions.VariableName(associationMember.ColumnName), systemType);
                    else
                        output = string.Format(", {0} {1}", systemType, NamingConventions.VariableName(associationMember.ColumnName));

                    if (!parameters.Contains(output))
                        parameters += output;
                }
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildVariableArguments(this List<Member> members)
        {
            string parameters = string.Empty;

            foreach (Member member in members)
            {
                parameters += string.Format(", {0}", member.VariableName);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildPropertyVariableArguments(this List<Member> members)
        {
            string parameters = string.Empty;

            foreach (Member member in members)
            {
                parameters += string.Format(", {0}", member.PropertyName);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildCriteriaParametersVariables(this List<Member> members)
        {
            return BuildCriteriaParametersVariables(members, "criteria");
        }

        public static string BuildCriteriaParametersVariables(this List<Member> members, string criteria)
        {
            string parameters = string.Empty;

            foreach (Member member in members)
            {
                parameters += string.Format(", {0}.{1}", criteria, member.PropertyName);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildReadPropertyParametersVariables(this List<Member> members)
        {
            return BuildReadPropertyParametersVariables(members, new List<AssociationMember>());
        }

        public static string BuildReadPropertyParametersVariables(this List<Member> members, List<AssociationMember> remoteAssociations)
        {
            string parameters = string.Empty;

            foreach (Member member in members)
            {
                parameters += string.Format(", ReadProperty({0}Property)", member.PrivateMemberVariableName);
            }

            foreach (AssociationMember associationMember in remoteAssociations)
            {
                string output = string.Format(", ReadProperty({0}Property)",
                                              NamingConventions.PrivateMemberVariableName(associationMember.ResolveManyToOneNameConflict(associationMember.Entity)));

                if (!parameters.Contains(output))
                    parameters += output;
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildInsertReadPropertyParametersVariables(this List<Member> members)
        {
            return BuildInsertReadPropertyParametersVariables(members, new List<AssociationMember>());
        }

        public static string BuildInsertReadPropertyParametersVariables(this List<Member> members, List<AssociationMember> remoteAssociations)
        {
            string parameters = string.Empty;

            foreach (Member member in members)
            {
                if (!member.IsIdentity)
                    parameters += string.Format(", ReadProperty({0}Property)", member.Entity.ResolveCriteriaPrivateMemberVariableName(member.PrivateMemberVariableName));
            }

            foreach (AssociationMember associationMember in remoteAssociations)
            {
                string output = string.Format(", ReadProperty({0}Property)",
                                              NamingConventions.PrivateMemberVariableName(associationMember.ResolveManyToOneNameConflict(associationMember.Entity)));

                if (!parameters.Contains(output))
                    parameters += output;
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }


        public static string BuildPrivateMemberVariableParameters(this List<Member> members)
        {
            return BuildPrivateMemberVariableParameters(members, new List<AssociationMember>());
        }

        public static string BuildPrivateMemberVariableParameters(this List<Member> members, List<AssociationMember> remoteAssociations)
        {
            string parameters = string.Empty;

            foreach (Member member in members)
            {
                parameters += string.Format(", {0}", member.PrivateMemberVariableName);
            }

            foreach (AssociationMember associationMember in remoteAssociations)
            {
                string output = string.Format(", {0}",
                                              NamingConventions.PrivateMemberVariableName(associationMember.ResolveManyToOneNameConflict(associationMember.Entity)));

                if (!parameters.Contains(output))
                    parameters += output;
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildInsertPrivateMemberVariableParameters(this List<Member> members)
        {
            return BuildInsertPrivateMemberVariableParameters(members, new List<AssociationMember>());
        }

        public static string BuildInsertPrivateMemberVariableParameters(this List<Member> members, List<AssociationMember> remoteAssociations)
        {
            string parameters = string.Empty;

            foreach (Member member in members)
            {
                if (!member.IsIdentity)
                    parameters += string.Format(", {0}", member.PrivateMemberVariableName);
            }

            foreach (AssociationMember associationMember in remoteAssociations)
            {
                string output = string.Format(", {0}", NamingConventions.PrivateMemberVariableName(associationMember.ResolveManyToOneNameConflict(associationMember.Entity)));

                if (!parameters.Contains(output))
                    parameters += output;
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildDataBaseColumns(this List<Member> members)
        {
            string columnNames = string.Empty;

            foreach (Member member in members)
            {
                columnNames += string.Format(", [{0}]", member.ColumnName);
            }

            return columnNames.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildDataBaseParameters(this List<Member> members)
        {
            string columnNames = string.Empty;

            foreach (Member member in members)
            {
                columnNames += string.Format(", {0}{1}",  Configuration.Instance.ParameterPrefix, member.Name);
            }

            return columnNames.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildInsertDataBaseColumns(this List<Member> members)
        {
            return BuildInsertDataBaseColumns(members, new List<AssociationMember>());
        }

        public static string BuildInsertDataBaseColumns(this List<Member> members, List<AssociationMember> associationMembers)
        {
            string columnNames = string.Empty;

            foreach (Member member in members)
            {
                if (!member.IsIdentity)
                    columnNames += string.Format(", [{0}]", member.ColumnName);
            }

            foreach (AssociationMember associationMember in associationMembers)
            {
                if (!associationMember.IsIdentity)
                {
                    string output = string.Format(", [{0}]", associationMember.ColumnName);

                    if (!columnNames.Contains(output))
                        columnNames += output;
                }
            }

            return columnNames.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildInsertDataBaseParameters(this List<Member> members)
        {
            return BuildInsertDataBaseParameters(members, new List<AssociationMember>());
        }

        public static string BuildInsertDataBaseParameters(this List<Member> members, List<AssociationMember> associationMembers)
        {
            string columnNames = string.Empty;

            foreach (Member member in members)
            {
                if (!member.IsIdentity)
                    columnNames += string.Format(", {0}{1}", Configuration.Instance.ParameterPrefix, member.Name);
            }

            foreach (AssociationMember associationMember in associationMembers)
            {
                if (!associationMember.IsIdentity)
                {
                    string output = string.Format(", {0}{1}", Configuration.Instance.ParameterPrefix,
                                                  associationMember.ColumnName);

                    if (!columnNames.Contains(output))
                        columnNames += output;
                }
            }

            return columnNames.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildInsertCommandParameters(this List<Member> members)
        {
            return BuildInsertCommandParameters(members, new List<AssociationMember>());
        }

        public static string BuildInsertCommandParameters(this List<Member> members, List<AssociationMember> associationMembers)
        {
            string commandParameters = string.Empty;

            foreach (Member member in members)
            {
                if (member.IsIdentity)
                    continue;

                string cast;
                if (member.SystemType.Contains("SmartDate"))
                {
                    if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
                        cast = member.IsNullable ? string.Format("IIf({0}.HasValue, DirectCast({0}.Value.Date, DateTime), System.DBNull.Value))", member.PropertyName)
                                                 : string.Format("DirectCast({0}.Date, DateTime))", member.PropertyName);
                    else
                        cast = member.IsNullable ? string.Format("(DateTime?){0});", member.PropertyName)
                                                 : string.Format("(DateTime){0});", member.PropertyName);
                }
                else
                    cast = Configuration.Instance.TargetLanguage == LanguageEnum.VB ? string.Format("{0})", member.PropertyName) : string.Format("{0});", member.PropertyName);

                commandParameters += string.Format("\n\t\t\t\t\t\tcommand.Parameters.AddWithValue(\"{0}{1}\", {2}", Configuration.Instance.ParameterPrefix, member.ColumnName, cast);
            }

            foreach (AssociationMember associationMember in associationMembers)
            {
                if (!associationMember.IsIdentity)
                {
                    string cast;
                    string propertyName = string.Format("{0}.{1}", associationMember.VariableName, NamingConventions.PropertyName(associationMember.LocalColumn.Name));
                    if (associationMember.SystemType.Contains("SmartDate"))
                    {
                        if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
                            cast = associationMember.IsNullable ? string.Format("IIf({0}.HasValue, DirectCast({0}.Value.Date, DateTime), System.DBNull.Value))", propertyName)
                                                                : string.Format("DirectCast({0}.Date, DateTime))", propertyName);
                        else
                            cast = associationMember.IsNullable ? string.Format("(DateTime?){0});", propertyName)
                                                                : string.Format("(DateTime){0});", propertyName);
                    }
                    else
                        cast = Configuration.Instance.TargetLanguage == LanguageEnum.VB ? string.Format("{0})", propertyName) : string.Format("{0});", propertyName);

                    string output = string.Format("\n\t\t\t\t\t\tcommand.Parameters.AddWithValue(\"{0}{1}\", {2}", Configuration.Instance.ParameterPrefix, associationMember.ColumnName, cast);

                    if (!commandParameters.Contains(output))
                        commandParameters += output;
                }
            }

            return commandParameters.TrimStart(new[] { '\t', '\n' });
        }

        public static string BuildCommandParameters(this List<Member> members)
        {
            return BuildCommandParameters(members, new List<AssociationMember>());
        }

        public static string BuildCommandParameters(this List<Member> members, List<AssociationMember> associationMembers)
        {
            string commandParameters = string.Empty;

            foreach (Member member in members)
            {
                string cast;
                if (member.SystemType.Contains("SmartDate"))
                {
                    if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
                        cast = member.IsNullable ? string.Format("IIf({0}.HasValue, DirectCast({0}.Value.Date, DateTime), System.DBNull.Value))", member.PropertyName)
                                                 : string.Format("DirectCast({0}.Date, DateTime))", member.PropertyName);
                    else
                        cast = member.IsNullable ? string.Format("(DateTime?){0});", member.PropertyName)
                                                 : string.Format("(DateTime){0});", member.PropertyName);
                }
                else
                    cast = Configuration.Instance.TargetLanguage == LanguageEnum.VB ? string.Format("{0})", member.PropertyName) : string.Format("{0});", member.PropertyName);

                commandParameters += string.Format("\n\t\t\t\t\t\tcommand.Parameters.AddWithValue(\"{0}{1}\", {2}", Configuration.Instance.ParameterPrefix, member.ColumnName, cast);
            }

            foreach (AssociationMember associationMember in associationMembers)
            {
                string cast;
                string propertyName = NamingConventions.PropertyName(associationMember.ColumnName);
                if (associationMember.SystemType.Contains("SmartDate"))
                {
                    if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
                        cast = associationMember.IsNullable ? string.Format("IIf({0}.HasValue, DirectCast({0}.Value.Date, DateTime), System.DBNull.Value))", propertyName)
                                                            : string.Format("DirectCast({0}.Date, DateTime))", propertyName);
                    else
                        cast = associationMember.IsNullable ? string.Format("(DateTime?){0});", propertyName)
                                                            : string.Format("(DateTime){0});", propertyName);
                }
                else
                    cast = Configuration.Instance.TargetLanguage == LanguageEnum.VB ? string.Format("{0})", propertyName) : string.Format("{0});", propertyName);

                string output = string.Format("\n\t\t\t\t\tcommand.Parameters.AddWithValue(\"{0}{1}\", {2}", Configuration.Instance.ParameterPrefix, associationMember.ColumnName, cast);

                if (!commandParameters.Contains(output))
                    commandParameters += output;
            }

            return commandParameters.TrimStart(new[] { '\t', '\n' });
        }

        public static string BuildSetStatements(this List<Member> members)
        {
            return BuildSetStatements(members, new List<AssociationMember>());
        }

        public static string BuildSetStatements(this List<Member> members, List<AssociationMember> associationMembers)
        {
            string setStatements = "\t\t\t\t\t\t SET";

            foreach (Member member in members)
            {
                string name = member.ColumnName;
                string varname = member.VariableName;

                foreach (AssociationMember association in member.Entity.OneToMany)
                {
                    if (association.LocalColumn.ColumnName == member.ColumnName)
                    {
                        name = association.ColumnName;
                        break;
                    }
                }

                setStatements += string.Format(" [{0}] = {1}{2},", name, Configuration.Instance.ParameterPrefix, name);
            }

            foreach (AssociationMember associationMember in associationMembers)
            {
                string output = string.Format(" [{0}] = {1}{2},", associationMember.ColumnName, Configuration.Instance.ParameterPrefix, NamingConventions.VariableName(associationMember.ColumnName));

                if (!setStatements.Contains(output))
                    setStatements += output;
            }

            return setStatements.TrimStart(new[] { '\t', '\n' }).TrimEnd(new[] { ',' });
        }

        public static string BuildWhereStatements(this List<Member> members)
        {
            string columnNames = string.Empty;

            foreach (Member member in members)
            {
                columnNames += string.Format("[{0}] = {1} AND ", member.ColumnName, member.BuildParameterVariableName());
            }

            return string.Format("WHERE {0}", columnNames.Remove(columnNames.Length - 5, 5));
        }

        public static string BuildManyToOneWhereSubStatements(this List<Member> members, List<Member> primaryKeys)
        {
            string columnNames = string.Empty;

            foreach (Member member in members)
            {
                foreach (Member primaryKey in primaryKeys)
                {
                    columnNames += string.Format(" [{0}] in (SELECT [{1}] FROM [{2}].[{3}] {4}) AND", member.ColumnName, member.ColumnName, primaryKey.Entity.Table.Owner, primaryKey.Entity.Table.Name, primaryKey.BuildWhereStatement());
                }
            }

            return string.Format("WHERE {0}", columnNames.TrimStart(new[] { ' ' }).TrimEnd( new[] { 'A', 'N', 'D' }));
        }

        public static string BuildReaderStatements(this List<Member> members)
        {
            string columnNames = string.Empty;

            foreach (Member member in members)
            {
                columnNames += string.Format(", reader.{0}(\"{1}\")", member.GetReaderMethod(), member.Name);
            }

            return columnNames.TrimStart(new[] { ',', ' ' });
        }

        public static bool HasByteArrayColumn(this List<Member> members)
        {
            foreach (Member member in members)
            {
                if (member.HasByteArrayColumn())
                    return true;
            }

            return false;
        }
    }
}
