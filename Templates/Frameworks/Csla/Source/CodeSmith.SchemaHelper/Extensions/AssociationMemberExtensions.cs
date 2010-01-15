using System;
using System.Collections.Generic;
using System.Linq;
using CodeSmith.Engine;
using CodeSmith.SchemaHelper.Util;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for AssociationMemberCollectionExtensions
    /// </summary>
    public static class AssociationMemberCollectionExtensions
    {
        public static List<SearchCriteria> ListSearchCriteria(this AssociationMember member)
        {
            return member.SearchCriteria.Where(sc => !sc.IsUniqueResult && sc.MethodName.Contains(member.Name)).ToList();
        }

        public static string BuildObjectInitializer(this AssociationMember associationMember)
        {
            foreach (var member in associationMember.AssociationEntity().GetUniqueSearchCriteriaMembers())
            {
                if (member.ColumnName == associationMember.ColumnName)
                {
                    return string.Format("{0} = {1}", NamingConventions.PropertyName(member.ColumnName), NamingConventions.VariableName(member.ColumnName));
                }
            }

            var output = associationMember.BuildCriteriaObjectInitializer(associationMember.TableName);

            if (!output.StartsWith(associationMember.Entity.ResolveCriteriaVariableName(associationMember.ColumnName), StringComparison.InvariantCultureIgnoreCase))
                return string.Format("{0} = {1}", associationMember.Entity.ResolveCriteriaPropertyName(associationMember.ColumnName), associationMember.Entity.ResolveCriteriaVariableName(associationMember.ColumnName));

            return output;
        }

        public static string BuildParametersVariable(this AssociationMember associationMember)
        {
            foreach (var member in associationMember.AssociationEntity().GetUniqueSearchCriteriaMembers())
            {
                if (member.ColumnName == associationMember.ColumnName)
                {
                    if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
                    {
                        return string.Format("ByVal {0} As {1}", NamingConventions.VariableName(associationMember.ColumnName), associationMember.SystemType);
                    }

                    return string.Format("{0} {1}", associationMember.SystemType, NamingConventions.VariableName(associationMember.ColumnName));
                }
            }

            return associationMember.BuildParametersVariablesCriteria(false);
        }


        public static string BuildOneToZeroOrZeroObjectInitializer(this AssociationMember associationMember)
        {
            foreach (var member in associationMember.Entity.GetUniqueSearchCriteriaMembers())
            {
                if (member.ColumnName == associationMember.LocalColumn.ColumnName)
                {
                    return string.Format("{0} = {1}", NamingConventions.PropertyName(associationMember.LocalColumn.Name), NamingConventions.VariableName(associationMember.LocalColumn.Name));
                }
            }

            var output = associationMember.BuildCriteriaObjectInitializer(associationMember.TableName);

            if (!output.StartsWith(associationMember.Entity.ResolveCriteriaVariableName(associationMember.LocalColumn.Name), StringComparison.InvariantCultureIgnoreCase))
                return string.Format("{0} = {1}", associationMember.Entity.ResolveCriteriaPropertyName(associationMember.LocalColumn.Name), associationMember.Entity.ResolveCriteriaVariableName(associationMember.LocalColumn.Name));

            return output;
        }

        public static string BuildOneToZeroOrZeroParametersVariable(this AssociationMember associationMember)
        {
            foreach (var member in associationMember.AssociationEntity().GetUniqueSearchCriteriaMembers())
            {
                if (member.ColumnName == associationMember.ColumnName)
                {
                    if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
                    {
                        return string.Format("ByVal {0} As {1}", NamingConventions.VariableName(associationMember.LocalColumn.Name), associationMember.SystemType);
                    }

                    return string.Format("{0} {1}", associationMember.SystemType, NamingConventions.VariableName(associationMember.LocalColumn.Name));
                }
            }

            return associationMember.BuildParametersVariablesCriteria(false);
        }


        public static Entity AssociationEntity(this AssociationMember member)
        {
            return new Entity(member.Table);
        }

        public static List<SearchCriteria> AssociationEntityListSearchCriteria(this AssociationMember member)
        {
            return  member.AssociationEntity().SearchCriteria
                    .Where(sc =>
                        sc.MethodName.EndsWith(member.LocalColumn.Name) ||
                        sc.MethodName.EndsWith(member.LocalColumn.ColumnName) ||
                        sc.MethodName.EndsWith(member.Name) ||
                        sc.MethodName.EndsWith(member.ColumnName) &&
                        !sc.IsUniqueResult)
                    .ToList();
        }

        public static string ResolveManyToOneNameConflict(this AssociationMember member, Entity entity)
        {
            string propertyName = Util.NamingConventions.PropertyName(member.ColumnName);

            foreach (AssociationMember association in entity.ManyToOne)
            {
                if(association.PropertyName == propertyName)
                {
                    var name = entity.ResolveCriteriaColumnName(member.ColumnName);
                    if (string.Equals(name, member.Table.ClassName(), StringComparison.InvariantCultureIgnoreCase))
                        return string.Format("{0}Member", name);
                }
            }

            if (string.Equals(member.ColumnName, member.Table.ClassName(), StringComparison.InvariantCultureIgnoreCase))
                return string.Format("{0}Member", member.ColumnName);

            return member.ColumnName;
        }

        public static bool HasByteArrayColumn(this AssociationMember member)
        {
            if (member.Table.Columns.Contains(member.LocalColumn.Name))
                return DbTypeToDataReaderMethod[member.Table.Columns[member.LocalColumn.Name].DataType.ToString(), "GetValue"] == "GetBytes" || member.IsRowVersion;

            if (member.Table.Columns.Contains(member.ColumnName))
                return DbTypeToDataReaderMethod[member.Table.Columns[member.ColumnName].DataType.ToString(), "GetValue"] == "GetBytes" || member.IsRowVersion;

            if (member.AssociationEntity().Table.Columns.Contains(member.LocalColumn.Name))
                return DbTypeToDataReaderMethod[member.AssociationEntity().Table.Columns[member.LocalColumn.Name].DataType.ToString(), "GetValue"] == "GetBytes" || member.IsRowVersion;

            if (member.AssociationEntity().Table.Columns.Contains(member.ColumnName))
                return DbTypeToDataReaderMethod[member.AssociationEntity().Table.Columns[member.ColumnName].DataType.ToString(), "GetValue"] == "GetBytes" || member.IsRowVersion;

            return false;
        }

        #region Internal Properties and Members

        internal static MapCollection _dbTypeToDataReaderMethod;
        internal static MapCollection DbTypeToDataReaderMethod
        {
            get
            {
                if (_dbTypeToDataReaderMethod == null)
                {
                    string path;
                    if (Map.TryResolvePath("DbType-DataReaderMethod", "", out path))
                        _dbTypeToDataReaderMethod = Map.Load(path);
                }
                return _dbTypeToDataReaderMethod;
            }
        }

        #endregion
    }
}